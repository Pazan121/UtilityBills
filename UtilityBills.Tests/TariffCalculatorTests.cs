using UtilityBills.Core.Billing;
using Xunit;

namespace UtilityBills.Tests;

public class TariffCalculatorTests
{
    [Theory]
    [InlineData(0, false, 0)]
    [InlineData(0.01, false, 0.03)]
    [InlineData(50, false, 132.00)]
    [InlineData(50, true, 66.00)]
    [InlineData(50.123, false, 132.33)]
    [InlineData(100, false, 264.00)]
    [InlineData(100, true, 132.00)]
    [InlineData(100.5, false, 265.65)]
    [InlineData(101, false, 267.30)]
    [InlineData(101, true, 135.30)]
    [InlineData(130, false, 363.00)]
    [InlineData(130, true, 231.00)]
    [InlineData(600, false, 1914.00)]
    [InlineData(600, true, 1782.00)]
    [InlineData(601, false, 1918.20)]
    [InlineData(700, false, 2334.00)]
    [InlineData(700, true, 2202.00)]
    public void ChargeFor_StepsTable_MatchesDecisionTable(
        double consumption, bool isPrivileged, double expected)
    {
        // Act
        decimal actual = TariffCalculator.ChargeFor((decimal)consumption, isPrivileged);

        // Assert
        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-100)]
    public void ChargeFor_NegativeConsumption_ThrowsOutOfRange(double consumption)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TariffCalculator.ChargeFor((decimal)consumption, false));
    }

    [Fact]
    public void Steps_ZeroConsumption_ReturnsNoSteps()
    {
        var steps = TariffCalculator.Steps(0m, false);

        Assert.Empty(steps);
    }

    [Fact]
    public void Steps_ConsumptionOnThirdStep_ReturnsThreeSteps()
    {
        var steps = TariffCalculator.Steps(700m, false);

        Assert.Equal(3, steps.Count);
        Assert.Equal(TariffCalculator.FirstStepLimit, steps[0].Quantity);
        Assert.Equal(500m, steps[1].Quantity);
        Assert.Equal(100m, steps[2].Quantity);
    }

    [Fact]
    public void Steps_PrivilegedConsumer_HalvesOnlyFirstStepTariff()
    {
        var steps = TariffCalculator.Steps(130m, true);

        Assert.Equal(1.32m, steps[0].Tariff);
        Assert.Equal(TariffCalculator.SecondStepTariff, steps[1].Tariff);
    }
}
