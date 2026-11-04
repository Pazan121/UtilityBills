using System.Text;

namespace UtilityBills.Core.Reporting;

/// <summary>Формує текст простого звіту з готових рядків.</summary>
public sealed class ReportBuilder
{
    /// <summary>Initializes a new instance of the <see cref="ReportBuilder"/> class.</summary>
    /// <param name="title">Заголовок звіту.</param>
    public ReportBuilder(string title = "Звіт")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        Title = title;
    }

    /// <summary>Gets the заголовок звіту.</summary>
    public string Title { get; }

    /// <summary>Будує звіт: заголовок великими літерами та рядки по одному на рядок.</summary>
    /// <param name="rows">Готові рядки звіту.</param>
    /// <returns>Текст звіту.</returns>
    public string Build(IReadOnlyList<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        var text = new StringBuilder();
        text.AppendLine(Title.ToUpperInvariant());
        foreach (var row in rows)
        {
            text.AppendLine(row);
        }

        return text.ToString();
    }

        public string Describe(int count)
    {
        string note;
        return "Позицій: " + count;
    }
}
