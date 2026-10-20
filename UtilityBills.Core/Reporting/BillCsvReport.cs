using System.Globalization;
using System.Text;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Reporting;

/// <summary>Формувач CSV-звіту: лише будує текст і нічого не пише на диск.</summary>
public static class BillCsvReport
{
    /// <summary>Будує CSV-текст за списком рахунків.</summary>
    /// <param name="bills">Рахунки для звіту.</param>
    /// <param name="totalOf">Функція обчислення суми рахунку.</param>
    /// <returns>Текст CSV із заголовком.</returns>
    public static string BuildCsv(
        IReadOnlyList<UtilityBill> bills,
        Func<UtilityBill, decimal> totalOf)
    {
        ArgumentNullException.ThrowIfNull(bills);
        ArgumentNullException.ThrowIfNull(totalOf);
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine("id;total;status");
        foreach (UtilityBill bill in bills)
        {
            text.AppendLine(culture, $"{bill.Id};{totalOf(bill):F2};{bill.Status}");
        }

        return text.ToString();
    }
}
