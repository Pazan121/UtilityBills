using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Storage;

/// <summary>Сховище рахунків у пам'яті на словнику за ключем Id.</summary>
public sealed class InMemoryBillRepository : IBillRepository
{
    private readonly Dictionary<int, UtilityBill> _bills = new();

    /// <inheritdoc/>
    public void Add(UtilityBill bill)
    {
        ArgumentNullException.ThrowIfNull(bill);
        if (!_bills.TryAdd(bill.Id, bill))
        {
            throw new InvalidOperationException(
                $"Рахунок {bill.Id} уже існує.");
        }
    }

    /// <inheritdoc/>
    public UtilityBill? GetById(int id)
    {
        return _bills.TryGetValue(id, out UtilityBill? bill) ? bill : null;
    }

    /// <inheritdoc/>
    public IReadOnlyList<UtilityBill> GetAll()
    {
        return _bills.Values.ToList();
    }
}
