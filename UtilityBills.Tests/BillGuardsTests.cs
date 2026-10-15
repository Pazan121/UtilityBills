using UtilityBills.Core.Billing;
using Xunit;

namespace UtilityBills.Tests;

public class BillGuardsTests
{
    [Fact]
    public void Calculate_NoLines_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() =>
            BillGuards.EnsureLinesValid(new List<ChargeLine>()));
    }

    [Fact]
    public void Calculate_ZeroQuantity_ThrowsRange()
    {
        ChargeLine line = new()
        {
            ServiceCode = "WATER",
            Quantity = 0,
            Tariff = 10m,
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => BillGuards.EnsureLineValid(line));
    }

    [Fact]
    public void EnsureHeaderValid_EmailWithoutAt_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(
            () => BillGuards.EnsureHeaderValid(1, "Іваненко", "no-at.example.com"));
    }
}
