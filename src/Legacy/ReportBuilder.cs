using System.Text;

namespace UtilityBills.Legacy;

public sealed class ReportBuilder
{
    public string Build(int billId, string consumerName,
        IReadOnlyList<ServiceLine> lines, string currency,
        decimal discount, decimal shipping, decimal total)
    {
        var sb = new StringBuilder();
        sb.Append("Документ #").Append(billId).Append('\n');
        sb.Append("Клієнт: ").Append(consumerName).Append('\n');
        foreach (var line in lines)
        {
            sb.Append(Line(line, currency));
        }
        sb.Append(Money("Знижка", discount, currency));
        sb.Append(Money("Доставка", shipping, currency));
        sb.Append(Money("Разом", total, currency));
        return sb.ToString();
    }

    private static string Line(ServiceLine line, string currency) =>
        line.ServiceCode + " x" + line.Quantity + " = " +
        (line.Quantity * line.Tariff).ToString("0.00") + " " +
        currency + "\n";

    private static string Money(string label, decimal value,
        string currency) =>
        label + ": " + value.ToString("0.00") + " " + currency + "\n";
}
