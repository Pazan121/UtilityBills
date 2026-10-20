using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Pricing;
using UtilityBills.Core.Services;
using UtilityBills.Core.Storage;
using Xunit;

namespace UtilityBills.Tests;

public class DomainInvariantTests
{
    [Fact]
    public void UtilityBillLine_WithZeroQuantity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new UtilityBillLine(1, 0m, 10m));
    }

    [Fact]
    public void UtilityBill_AfterPay_RejectsNewLines()
    {
        UtilityBill bill = NewBill(1);
        bill.AddLine(new UtilityBillLine(1, 1m, 10m));
        bill.Issue();
        bill.Pay();

        Assert.Throws<InvalidOperationException>(
            () => bill.AddLine(new UtilityBillLine(2, 1m, 5m)));
    }

    [Fact]
    public void TotalOf_WithTenPercentDiscount_Returns180()
    {
        BillService service = new(
            new InMemoryBillRepository(),
            new DiscountPricingPolicy(0.10m),
            new SilentNotifier());
        UtilityBill bill = NewBill(2);
        bill.AddLine(new UtilityBillLine(1, 2m, 100m));

        Assert.Equal(180m, service.TotalOf(bill));
    }

    private static UtilityBill NewBill(int id)
    {
        return new UtilityBill(id, 100, "2026-10", new DateOnly(2026, 11, 20));
    }

    private sealed class SilentNotifier : INotifier
    {
        public void Notify(string recipient, string message)
        {
        }
    }
}
