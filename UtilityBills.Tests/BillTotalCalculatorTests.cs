// Файл UtilityBills.Tests/BillTotalCalculatorTests.cs у завданні 4
// (git mv LegacyProcessorTests.cs BillTotalCalculatorTests.cs).
using UtilityBills.Core.Billing;
using Xunit;

namespace UtilityBills.Tests;

public class BillTotalCalculatorTests
{
    [Fact]
    public void Process_RegularClient_Returns1292()
    {
        List<ChargeLine> lines = new()
        {
            new ChargeLine { ServiceCode = "WATER", Quantity = 12, Tariff = 100m },
            new ChargeLine { ServiceCode = "GAS", Quantity = 1, Tariff = 250m },
        };

        decimal total = BillTotalCalculator.Process(
            1, "Іваненко", "i@ex.com", true, lines,
            new DateOnly(2026, 3, 10), string.Empty, BillingStatus.Issued,
            false, 60m, out BillingStatus next);

        Assert.Equal(1292.70m, total);
        Assert.Equal(BillingStatus.Paid, next);
    }

    [Fact]
    public void Process_SmallOrder_AddsDelivery()
    {
        List<ChargeLine> lines = new()
        {
            new ChargeLine { ServiceCode = "WATER", Quantity = 2, Tariff = 200m },
        };

        decimal total = BillTotalCalculator.Process(
            2, "Петренко", "p@ex.com", false, lines,
            new DateOnly(2026, 3, 10), "SALE10", BillingStatus.Paid,
            false, 60m, out BillingStatus next);

        Assert.Equal(420m, total);
        Assert.Equal(BillingStatus.Closed, next);
    }
}
