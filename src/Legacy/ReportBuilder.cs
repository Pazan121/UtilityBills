using System.Text;

namespace UtilityBills.Legacy;

public sealed class ReportBuilder
{
    public string Build(BillRequest req, decimal discount,
        decimal shipping, decimal total)
    {
        var sb = new StringBuilder();
        sb.Append("Документ #").Append(req.BillId).Append('\n');
        sb.Append("Клієнт: ").Append(req.Client.Name).Append('\n');
        foreach (var line in req.Lines!)
        {
            sb.Append(Line(line, req.Currency));
        }
        sb.Append(Money("Знижка", discount, req.Currency));
        sb.Append(Money("Доставка", shipping, req.Currency));
        sb.Append(Money("Разом", total, req.Currency));
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
