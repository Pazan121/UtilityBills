using Microsoft.Extensions.Logging.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;
using UtilityBills.Core.Services;
using Xunit;

namespace UtilityBills.Tests;

public class BillServiceTests
{
    [Fact]
    public void TotalOf_TwoLines_SumsPricesFromPolicy()
    {
        // Arrange
        var repository = new FakeBillRepository();
        var bill = new UtilityBill(7, 1, "2026-10", new DateOnly(2026, 11, 20));
        bill.AddLine(new UtilityBillLine(1, 2m, 10m));
        bill.AddLine(new UtilityBillLine(2, 1m, 30m));
        repository.Add(bill);
        var service = NewService(
            repository, new StubPricingPolicy(25m), new RecordingNotifier());
        UtilityBill stored = service.Load(7);

        // Act
        decimal actual = service.TotalOf(stored);

        // Assert
        Assert.Equal(50m, actual);
    }

    [Fact]
    public void Place_BillWithLines_StoresAndNotifiesOnce()
    {
        // Arrange
        var repository = new FakeBillRepository();
        var notifier = new RecordingNotifier();
        var service = NewService(
            repository, new StubPricingPolicy(1m), notifier);
        var subscriber = new Subscriber(5, "Іван Іваненко", "Київ", "ivan@example.com");
        var bill = new UtilityBill(1, 5, "2026-10", new DateOnly(2026, 11, 20));
        bill.AddLine(new UtilityBillLine(1, 1m, 5m));

        // Act
        service.Place(bill, subscriber);

        // Assert
        Assert.NotNull(repository.GetById(1));
        Assert.Equal(1, repository.AddCalls);
        Assert.Single(notifier.Messages);
    }

    [Fact]
    public void Load_UnknownId_ThrowsDomainRule()
    {
        // Arrange
        var service = NewService(
            new FakeBillRepository(),
            new StubPricingPolicy(1m),
            new RecordingNotifier());

        // Act
        var ex = Assert.Throws<DomainRuleException>(
            () => service.Load(99));

        // Assert
        Assert.Equal("bill.exists", ex.Rule);
    }

    private static BillService NewService(
        FakeBillRepository repository,
        StubPricingPolicy pricing,
        RecordingNotifier notifier)
    {
        return new BillService(
            repository, pricing, notifier, NullLogger<BillService>.Instance);
    }
}
