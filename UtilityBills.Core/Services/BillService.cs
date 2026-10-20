using System.Globalization;
using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Services;

/// <summary>Прикладний сервіс: сценарій виставлення рахунку та розрахунок суми.</summary>
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
}
