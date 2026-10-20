using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Services;

/// <summary>Прикладний сервіс: сценарій виставлення рахунку та розрахунок суми.</summary>
public sealed class BillService
{
    private readonly IBillRepository _repository;
    private readonly IPricingPolicy _pricing;

    /// <summary>Initializes a new instance of the <see cref="BillService"/> class.</summary>
    /// <param name="repository">Сховище рахунків.</param>
    /// <param name="pricing">Політика ціноутворення.</param>
    public BillService(
        IBillRepository repository,
        IPricingPolicy pricing)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(pricing);
        _repository = repository;
        _pricing = pricing;
    }

    /// <summary>Виставляє рахунок і зберігає його.</summary>
    /// <param name="bill">Рахунок у стані чернетки.</param>
    public void Place(UtilityBill bill)
    {
        ArgumentNullException.ThrowIfNull(bill);
        bill.Issue();
        _repository.Add(bill);
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
