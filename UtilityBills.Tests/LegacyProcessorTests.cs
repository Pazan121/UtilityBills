using UtilityBills.Core.Billing;
using UtilityBills.Core.Legacy;
using Xunit;

namespace UtilityBills.Tests;

public class LegacyProcessorTests
{
    [Fact]
    public void Process_RegularClient_Returns1292()
    {
        List<ChargeLine> lines = new()
        {
            new ChargeLine { ServiceCode = "WATER", Quantity = 12, Tariff = 100m },
            new ChargeLine { ServiceCode = "GAS", Quantity = 1, Tariff = 250m },
        };

        decimal total = LegacyProcessor.Process(
            1, "Іваненко", "i@ex.com", true, lines,
            new DateOnly(2026, 3, 10), string.Empty, "Issued", false,
            60m, out string next);

        Assert.Equal(1292.70m, total);
        Assert.Equal("Paid", next);
    }

    [Fact]
    public void Process_SmallOrder_AddsDelivery()
    {
        List<ChargeLine> lines = new()
        {
            new ChargeLine { ServiceCode = "WATER", Quantity = 2, Tariff = 200m },
        };

        decimal total = LegacyProcessor.Process(
            2, "Петренко", "p@ex.com", false, lines,
            new DateOnly(2026, 3, 10), "SALE10", "Paid",
            false, 60m, out string next);

        Assert.Equal(420m, total);
        Assert.Equal("Closed", next);
    }

    [Fact]
    public void Process_NoLines_ReturnsMinusOne()
    {
        decimal total = LegacyProcessor.Process(
            3, "Коваль", "k@ex.com", false,
            new List<ChargeLine>(),
            new DateOnly(2026, 3, 10), string.Empty, "Issued", false,
            60m, out string next);

        Assert.Equal(-1m, total);
        Assert.Equal("Issued", next);
    }
}
