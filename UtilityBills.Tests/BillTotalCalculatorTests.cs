using UtilityBills.Core.Billing;
using Xunit;

namespace UtilityBills.Tests;

public class BillTotalCalculatorTests
{
    [Fact]
    public void Calculate_RegularClient_Returns1292()
    {
        BillTotalRequest request = new(
            BillId: 1,
            Subscriber: new SubscriberInfo("Іваненко", "i@ex.com", true),
            Lines: new List<ChargeLine>
            {
                new() { ServiceCode = "WATER", Quantity = 12, Tariff = 100m },
                new() { ServiceCode = "GAS", Quantity = 1, Tariff = 250m },
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: null,
            Status: BillingStatus.Issued,
            DeliveryPrice: 60m);

        BillTotalResult result = BillTotalCalculator.Calculate(request);

        Assert.Equal(1292.70m, result.Total);
        Assert.Equal(BillingStatus.Paid, result.Status);
    }

    [Fact]
    public void Calculate_SmallOrder_AddsDelivery()
    {
        BillTotalRequest request = RequestOf(
            new() { ServiceCode = "WATER", Quantity = 2, Tariff = 200m },
            new DateOnly(2026, 3, 10),
            "SALE10",
            BillingStatus.Paid);

        BillTotalResult result = BillTotalCalculator.Calculate(request);

        Assert.Equal(420m, result.Total);
        Assert.Equal(BillingStatus.Closed, result.Status);
    }

    [Fact]
    public void Calculate_CancelledBill_ReturnsZero()
    {
        BillTotalRequest request = RequestOf(
            new() { ServiceCode = "WATER", Quantity = 1, Tariff = 100m },
            new DateOnly(2026, 3, 10),
            null,
            BillingStatus.Cancelled);

        BillTotalResult result = BillTotalCalculator.Calculate(request);

        Assert.Equal(0m, result.Total);
        Assert.Equal(BillingStatus.Cancelled, result.Status);
    }

    [Fact]
    public void Calculate_SundayLargeBill_AppliesVolumeAndWeekend()
    {
        BillTotalRequest request = RequestOf(
            new() { ServiceCode = "HEAT", Quantity = 1, Tariff = 6000m },
            new DateOnly(2026, 3, 8),
            null,
            BillingStatus.Issued);

        BillTotalResult result = BillTotalCalculator.Calculate(request);

        Assert.Equal(5292m, result.Total);
        Assert.Equal(BillingStatus.Paid, result.Status);
    }

    [Fact]
    public void Calculate_FixedCoupon_SubtractsTwoHundred()
    {
        BillTotalRequest request = RequestOf(
            new() { ServiceCode = "HEAT", Quantity = 1, Tariff = 1500m },
            new DateOnly(2026, 3, 10),
            "MINUS200",
            BillingStatus.Issued);

        BillTotalResult result = BillTotalCalculator.Calculate(request);

        Assert.Equal(1300m, result.Total);
    }

    [Fact]
    public void Calculate_NullRequest_ThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => BillTotalCalculator.Calculate(null!));
    }

    private static BillTotalRequest RequestOf(
        ChargeLine line,
        DateOnly createdAt,
        string? couponCode,
        BillingStatus status)
    {
        return new BillTotalRequest(
            BillId: 1,
            Subscriber: new SubscriberInfo("Петренко", "p@ex.com", false),
            Lines: new List<ChargeLine> { line },
            CreatedAt: createdAt,
            CouponCode: couponCode,
            Status: status,
            DeliveryPrice: 60m);
    }
}
