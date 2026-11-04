using UtilityBills.Core.Reporting;
using Xunit;

namespace UtilityBills.Tests;

public class ReportBuilderTests
{
    // 1. Виносимо масив у приватне статичне поле
    private static readonly string[] _sampleRows = ["Вода: 100", "Газ: 50"];

    [Fact]
    public void Build_TwoRows_UppercasesTitleAndListsRows()
    {
        var builder = new ReportBuilder("Рахунок");

        var text = builder.Build(_sampleRows);

        Assert.StartsWith("РАХУНОК", text, StringComparison.Ordinal);
        Assert.Contains("Вода: 100", text, StringComparison.Ordinal);
        Assert.Contains("Газ: 50", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Build_NullRows_ThrowsArgumentNull()
    {
        var builder = new ReportBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.Build(null!));
    }

    [Fact]
    public void Constructor_EmptyTitle_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() => new ReportBuilder(" "));
    }
}
