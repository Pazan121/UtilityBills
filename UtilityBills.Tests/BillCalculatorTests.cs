using UtilityBills.Core.Billing;
using Xunit;

namespace UtilityBills.Tests;

public class BillCalculatorTests
{
    [Fact]
    public void ChargeFor_SmallBillNotPrivileged_ReturnsAmount()
    {
        // Arrange
        var calculator = new BillCalculator();

        // Act
        decimal actual = calculator.ChargeFor(400m, false);

        // Assert
        Assert.Equal(400m, actual);
    }

    [Fact]
    public void ChargeFor_NegativeAmount_ThrowsOutOfRange()
    {
        // Arrange
        var calculator = new BillCalculator();

        // Act
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.ChargeFor(-0.01m, true));

        // Assert
        Assert.Equal("amount", ex.ParamName);
    }

    [Fact]
    public void ChargeFor_ZeroAmount_ReturnsZero()
    {
        // Arrange
        var calculator = new BillCalculator();

        // Act
        decimal actual = calculator.ChargeFor(0m, false);

        // Assert
        Assert.Equal(0m, actual);
    }

    [Fact]
    public void ChargeFor_SmallBillPrivileged_AppliesPrivilegedRate()
    {
        // Arrange
        var calculator = new BillCalculator();

        // Act
        decimal actual = calculator.ChargeFor(400m, true);

        // Assert
        Assert.Equal(380m, actual);
    }

    [Fact]
    public void ChargeFor_JustBelowBigBill_AppliesNoBigRate()
    {
        // Arrange
        var calculator = new BillCalculator();

        // Act
        decimal actual = calculator.ChargeFor(999.99m, false);

        // Assert
        Assert.Equal(999.99m, actual);
    }

    [Fact]
    public void ChargeFor_ExactlyBigBill_AppliesBigRate()
    {
        // Arrange
        var calculator = new BillCalculator();

        // Act
        decimal actual = calculator.ChargeFor(1000m, false);

        // Assert
        Assert.Equal(900m, actual);
    }

    [Fact]
    public void ChargeFor_ExactlyBigBillPrivileged_AppliesBothRates()
    {
        // Arrange
        var calculator = new BillCalculator();

        // Act
        decimal actual = calculator.ChargeFor(1000m, true);

        // Assert
        Assert.Equal(850m, actual);
    }
}
