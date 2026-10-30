using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Tests;

internal sealed class FakeBillRepository : IBillRepository
{
    private readonly Dictionary<int, UtilityBill> _items = new();

    public int AddCalls { get; private set; }

    public void Add(UtilityBill bill)
    {
        AddCalls++;
        _items[bill.Id] = bill;
    }

    public UtilityBill? GetById(int id)
    {
        return _items.TryGetValue(id, out UtilityBill? found)
            ? found
            : null;
    }

    public IReadOnlyList<UtilityBill> GetAll()
    {
        return _items.Values.ToList();
    }
}

internal sealed class StubPricingPolicy : IPricingPolicy
{
    private readonly decimal _price;

    public StubPricingPolicy(decimal price)
    {
        _price = price;
    }

    public decimal PriceOf(UtilityBillLine line)
    {
        return _price;
    }
}

internal sealed class RecordingNotifier : INotifier
{
    public List<string> Messages { get; } = new();

    public void Notify(string recipient, string message)
    {
        Messages.Add(recipient + ": " + message);
    }
}
