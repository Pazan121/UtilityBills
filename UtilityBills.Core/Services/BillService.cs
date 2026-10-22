using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Logging;
using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;

namespace UtilityBills.Core.Services;

/// <summary>Прикладний сервіс: сценарії рахунку та розрахунок суми.</summary>
public sealed class BillService
{
    private readonly IBillRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<BillService> _log;

    /// <summary>Initializes a new instance of the <see cref="BillService"/> class.</summary>
    /// <param name="repository">Сховище рахунків.</param>
    /// <param name="pricing">Політика ціноутворення.</param>
    /// <param name="notifier">Спосіб сповіщення абонента.</param>
    /// <param name="log">Журнал подій сервісу.</param>
    public BillService(
        IBillRepository repository,
        IPricingPolicy pricing,
        INotifier notifier,
        ILogger<BillService> log)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(pricing);
        ArgumentNullException.ThrowIfNull(notifier);
        ArgumentNullException.ThrowIfNull(log);
        _repository = repository;
        _pricing = pricing;
        _notifier = notifier;
        _log = log;
    }

    /// <summary>Рахує рядки, що є цілими числами більше нуля.</summary>
    /// <param name="rows">Рядки введення.</param>
    /// <returns>Кількість коректних значень.</returns>
    public static int CountValidQuantities(string[] rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        int valid = 0;
        foreach (string row in rows)
        {
            if (int.TryParse(
                    row,
                    NumberStyles.Integer,
                    CultureInfo.CurrentCulture,
                    out int quantity)
                && quantity > 0)
            {
                valid++;
            }
        }

        return valid;
    }

    /// <summary>Виставляє рахунок, зберігає його та сповіщає абонента.</summary>
    /// <param name="bill">Рахунок у стані чернетки.</param>
    /// <param name="subscriber">Абонент, якому належить рахунок.</param>
    /// <returns>Сума рахунку.</returns>
    public decimal Place(UtilityBill bill, Subscriber subscriber)
    {
        ArgumentNullException.ThrowIfNull(bill);
        ArgumentNullException.ThrowIfNull(subscriber);
        using IDisposable? scope = _log.BeginScope("Bill:{BillId}", bill.Id);
        if (subscriber.Id != bill.SubscriberId)
        {
            throw new ArgumentException(
                "Рахунок належить іншому абоненту.",
                nameof(subscriber));
        }

        if (bill.Lines.Count == 0)
        {
            throw new DomainRuleException(
                "bill.empty",
                "Рахунок не містить жодного рядка");
        }

        bill.Issue();
        if (_repository.GetById(bill.Id) is null)
        {
            _repository.Add(bill);
        }

        decimal total = TotalOf(bill);
        string amount = total.ToString("0.00", CultureInfo.CurrentCulture);
        _notifier.Notify(
            subscriber.Email,
            $"Рахунок № {bill.Id}: {amount} грн");
        _log.LogInformation("Рахунок виставлено на {Amount}", total);
        return total;
    }

    /// <summary>Додає типізований рядок до чернетки рахунку.</summary>
    /// <param name="billId">Номер рахунку.</param>
    /// <param name="line">Рядок рахунку.</param>
    public void AddLine(int billId, UtilityBillLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        UtilityBill bill = _repository.GetById(billId)
            ?? throw new DomainRuleException(
                "bill.exists",
                $"Рахунок {billId} не знайдено");
        if (bill.Status != BillStatus.Draft)
        {
            throw new DomainRuleException(
                "bill.editable",
                $"Стан {bill.Status} не дозволяє додавати рядки");
        }

        bill.AddLine(line);
    }

    /// <summary>Фіксує оплату рахунку; порушення правил — DomainRuleException.</summary>
    /// <param name="billId">Номер рахунку.</param>
    /// <param name="amount">Сплачена сума.</param>
    public void Pay(int billId, decimal amount)
    {
        using IDisposable? scope = _log.BeginScope("Bill:{BillId}", billId);
        _log.LogDebug("Початок оплати, сума {Amount}", amount);
        UtilityBill bill = _repository.GetById(billId)
            ?? throw new DomainRuleException(
                "bill.exists",
                $"Рахунок {billId} не знайдено");
        if (bill.Status != BillStatus.Issued && bill.Status != BillStatus.Overdue)
        {
            throw new DomainRuleException(
                "bill.payable",
                $"Стан {bill.Status} не дозволяє оплату");
        }

        decimal total = TotalOf(bill);
        if (amount < total)
        {
            throw new DomainRuleException(
                "bill.amount",
                $"Сплачено {amount}, треба {total}");
        }

        if (amount > total)
        {
            _log.LogWarning("Переплата {Extra}", amount - total);
        }

        bill.Pay();
        _log.LogInformation("Оплачено на {Amount}", amount);
    }

    /// <summary>Повертає рахунок або кидає DomainRuleException, якщо його немає.</summary>
    /// <param name="billId">Номер рахунку.</param>
    /// <returns>Знайдений рахунок.</returns>
    public UtilityBill Load(int billId)
    {
        return _repository.GetById(billId)
            ?? throw new DomainRuleException(
                "bill.exists",
                $"Рахунок {billId} не знайдено");
    }

    /// <summary>Скасовує рахунок; збій сховища логується й пробрасується далі.</summary>
    /// <param name="billId">Номер рахунку.</param>
    /// <param name="reason">Причина скасування.</param>
    public void Cancel(int billId, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        using IDisposable? scope = _log.BeginScope("Bill:{BillId}", billId);
        try
        {
            UtilityBill bill = Load(billId);
            if (bill.Status != BillStatus.Draft && bill.Status != BillStatus.Issued)
            {
                throw new DomainRuleException(
                    "bill.cancellable",
                    $"Стан {bill.Status} не дозволяє скасування");
            }

            bill.Cancel();
            _log.LogInformation("Рахунок скасовано, причина: {Reason}", reason);
        }
        catch (IOException ex)
        {
            _log.LogError(ex, "Збій сховища, {BillId}", billId);
            throw;
        }
    }

    /// <summary>Записує звіт у файл; потік закривається навіть при винятку.</summary>
    /// <param name="path">Шлях до файлу.</param>
    /// <param name="bill">Рахунок.</param>
    public void ExportReport(string path, UtilityBill bill)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(bill);
        try
        {
            using StreamWriter writer = new(path);
            writer.WriteLine($"Рахунок {bill.Id}");
            writer.WriteLine(TotalOf(bill).ToString("0.00", CultureInfo.CurrentCulture));
        }
        catch (IOException ex)
        {
            _log.LogError(ex, "Не вдалося записати звіт {Path}", path);
            throw;
        }
    }

    /// <summary>Обчислює суму рахунку за політикою ціноутворення.</summary>
    /// <param name="bill">Рахунок.</param>
    /// <returns>Сума рахунку.</returns>
    public decimal TotalOf(UtilityBill bill)
    {
        ArgumentNullException.ThrowIfNull(bill);
        decimal total = 0m;
        foreach (UtilityBillLine line in bill.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        Debug.Assert(total >= 0m, "Підсумок рахунку не може бути від’ємним");
        return total;
    }
}
