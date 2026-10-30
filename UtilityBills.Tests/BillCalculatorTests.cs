using UtilityBills.Core.Billing;
using Xunit;

namespace UtilityBills.Tests;

public class BillCalculatorTests
{
    [Theory]
    [InlineData(0, false, 0)]
    [InlineData(400, false, 400)]
    [InlineData(400, true, 380)]
    [InlineData(999.99, false, 999.99)]
    [InlineData(1000, false, 900)]
    [InlineData(1000, true, 850)]
    [InlineData(1000.01, false, 900.01)]
    [InlineData(1000.01, true, 850.01)]
    public void ChargeFor_Boundaries_MatchesTable(
        double amount, bool isPrivileged, double expected)
    {
        // Arrange
        var calculator = new BillCalculator();

        // Act
        decimal actual = calculator.ChargeFor(
            (decimal)amount, isPrivileged);

        // Assert
        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1000)]
    public void ChargeFor_NegativeAmount_ThrowsOutOfRange(
        double amount)
    {
        // Arrange
        var calculator = new BillCalculator();

        // Act
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.ChargeFor((decimal)amount, true));

        // Assert
        Assert.Equal("amount", ex.ParamName);
    }
}
