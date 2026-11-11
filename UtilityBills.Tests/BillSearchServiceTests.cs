using UtilityBills.Core.Domain;
using UtilityBills.Core.Services;
using UtilityBills.Core.Storage;
using Xunit;

namespace UtilityBills.Tests;

public class BillSearchServiceTests
{
    [Fact]
    public void Find_EmptyRepository_ReturnsEmptyList()
    {
        var service = new BillSearchService(new InMemoryBillRepository());

        var found = service.Find(null, null);

        Assert.Empty(found);
    }

    [Fact]
    public void Find_BySubscriber_ReturnsOnlyHisBills()
    {
        var service = NewServiceWithThreeBills();

        var found = service.Find(1, null);

        Assert.Equal(2, found.Count);
        Assert.All(found, bill => Assert.Equal(1, bill.SubscriberId));
    }

    [Fact]
    public void Find_BySubscriberAndStatus_AppliesBothCriteria()
    {
        var service = NewServiceWithThreeBills();

        var found = service.Find(1, BillStatus.Paid);

        Assert.Single(found);
        Assert.Equal(1, found[0].Id);
    }

    [Fact]
    public void Find_NoCriteria_ReturnsAllOrderedById()
    {
        var service = NewServiceWithThreeBills();

        var found = service.Find(null, null);

        Assert.Equal(_expectedBillIds, found.Select(bill => bill.Id).ToArray());
    }
    private static readonly int[] _expectedBillIds = [1, 2, 3];
    private static BillSearchService NewServiceWithThreeBills()
    {
        var repository = new InMemoryBillRepository();
        repository.Add(NewBill(3, 2, pay: false));
        repository.Add(NewBill(1, 1, pay: true));
        repository.Add(NewBill(2, 1, pay: false));
        return new BillSearchService(repository);
    }

    private static UtilityBill NewBill(int id, int subscriberId, bool pay)
    {
        var bill = new UtilityBill(id, subscriberId, "2026-10", new DateOnly(2026, 11, 20));
        bill.AddLine(new UtilityBillLine(1, 10m, 2.64m));
        bill.Issue();
        if (pay)
        {
            bill.Pay();
        }

        return bill;
    }
}
