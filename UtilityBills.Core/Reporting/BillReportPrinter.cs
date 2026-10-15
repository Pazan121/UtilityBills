namespace UtilityBills.Core.Reporting;

/// <summary>Команда виведення звіту в довільний потік.</summary>
public sealed class BillReportPrinter
{
    private readonly TextWriter _output;

    /// <summary>Initializes a new instance of the <see cref="BillReportPrinter"/> class.</summary>
    /// <param name="output">Потік виведення.</param>
    public BillReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Записує готовий текст звіту в потік.</summary>
    /// <param name="report">Текст звіту.</param>
    public void Print(string report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(report);
        _output.Write(report);
    }
}
