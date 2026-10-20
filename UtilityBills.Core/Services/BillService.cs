using System.Globalization;
using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Services;

/// <summary>Прикладний сервіс: сценарії рахунку та розрахунок суми.</summary>
public sealed class BillService
{
    private readonly IBillRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    /// <summary>Initializes a new instance of the <see cref="BillService"/> class.</summary>
    /// <param name="repository">Сховище рахунків.</param>
    /// <param name="pricing">Політика ціноутворення.</param>
    /// <param name="notifier">Спосіб сповіщення абонента.</param>
    public BillService(
        IBillRepository repository,
        IPricingPolicy pricing,
        INotifier notifier)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(pricing);
        ArgumentNullException.ThrowIfNull(notifier);
        _repository = repository;
        _pricing = pricing;
        _notifier = notifier;
    }

    /// <summary>Виставляє рахунок, зберігає його та сповіщає абонента.</summary>
    /// <param name="bill">Рахунок у стані чернетки.</param>
    /// <param name="subscriber">Абонент, якому належить рахунок.</param>
    /// <returns>Сума рахунку.</returns>
    public decimal Place(UtilityBill bill, Subscriber subscriber)
    {
        ArgumentNullException.ThrowIfNull(bill);
        ArgumentNullException.ThrowIfNull(subscriber);
        if (subscriber.Id != bill.SubscriberId)
        {
            throw new ArgumentException(
                "Рахунок належить іншому абоненту.",
                nameof(subscriber));
        }

        bill.Issue();
        _repository.Add(bill);
        decimal total = TotalOf(bill);
        string amount = total.ToString("0.00", CultureInfo.CurrentCulture);
        _notifier.Notify(
            subscriber.Email,
            $"Рахунок № {bill.Id}: {amount} грн");
        return total;
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

        return total;
    }

#pragma warning disable CA1031, CA1305, CA2000, CA2200, CS8600, CS8602, IDE0060, SA1204
    // АНТИПАТЕРН 4 (завдання 5): try/catch замість звичайної умови.
    public static int CountValidQuantities(string[] rows)
    {
        int valid = 0;
        foreach (string row in rows)
        {
            try
            {
                int quantity = int.Parse(row);
                if (quantity > 0)
                {
                    valid++;
                }
            }
            catch (FormatException)
            {
                // не число — пропускаємо
            }
        }

        return valid;
    }

    // АНТИПАТЕРН (завдання 2): ядро само розбирає рядки.
    public void AddLine(int billId, string serviceId, string quantity, string tariff)
    {
        UtilityBill bill = _repository.GetById(billId);
        decimal q = decimal.Parse(quantity);
        decimal p = decimal.Parse(tariff);
        bill.AddLine(new UtilityBillLine(int.Parse(serviceId), q, p));
    }

    // АНТИПАТЕРН (завдання 3): коди повернення без назв і сенсу.
    public int Pay(int billId, decimal amount)
    {
        UtilityBill? bill = _repository.GetById(billId);
        if (bill is null)
        {
            return -1;
        }

        if (bill.Status != BillStatus.Issued)
        {
            return -2;
        }

        if (amount < TotalOf(bill))
        {
            return -3;
        }

        bill.Pay();
        return 0;
    }

    // АНТИПАТЕРН 1 (завдання 5): порожній catch і повернення null.
    public UtilityBill? Load(int billId)
    {
        try
        {
            return _repository.GetById(billId);
        }
        catch (Exception)
        {
        }

        return null;
    }

    // АНТИПАТЕРНИ 2 і 3 (завдання 5): широке перехоплення і throw ex.
    public void Cancel(int billId, string reason)
    {
        try
        {
            UtilityBill bill = _repository.GetById(billId);
            bill.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Щось пішло не так");
            throw ex;
        }
    }

    // ПОМИЛКА РЕСУРСІВ (завдання 7): при винятку файл лишається відкритим.
    public void ExportReport(string path, UtilityBill bill)
    {
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine($"Рахунок {bill.Id}");
        writer.WriteLine(TotalOf(bill));
        writer.Close();
    }
#pragma warning restore CA1031, CA1305, CA2000, CA2200, CS8600, CS8602, IDE0060, SA1204
}
