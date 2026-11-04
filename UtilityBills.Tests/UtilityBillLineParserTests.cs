using UtilityBills.Core.Parsing;
using Xunit;

namespace UtilityBills.Tests;

public class UtilityBillLineParserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1;2")]
    [InlineData("1;2;10;5")]
    [InlineData("0;2;10.50")]
    [InlineData("10000;2;10.50")]
    [InlineData("abc;2;10.50")]
    [InlineData("1;0;10.50")]
    [InlineData("1;-2;10.50")]
    [InlineData("1;2;-10.50")]
    [InlineData("1;2;abc")]
    [InlineData("1;2;10,50")]
    public void TryParse_InvalidInput_ReturnsFalse(string? raw)
    {
        var ok = UtilityBillLineParser.TryParse(raw, out var line);

        Assert.False(ok);
        Assert.Null(line);
    }

    [Fact]
    public void TryParse_ValidInput_ReturnsLine()
    {
        var ok = UtilityBillLineParser.TryParse(" 5 ; 12.5 ; 24.75 ", out var line);

        Assert.True(ok);
        Assert.NotNull(line);
        Assert.Equal(5, line.UtilityServiceId);
        Assert.Equal(12.5m, line.Quantity);
        Assert.Equal(24.75m, line.Tariff);
    }
}
