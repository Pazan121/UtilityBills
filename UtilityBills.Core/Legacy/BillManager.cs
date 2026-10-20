#pragma warning disable CS1591, IDE0011, SA1503, SA1402, SA1649, CA1002, CA1822, CA1051
namespace UtilityBills.Core.Legacy;

/// <summary>Клас-Бог: п'ять порушень SOLID в одному місці (вихідний код аудиту).</summary>
public class BillManager
{
    private readonly FileBillStore _store = new FileBillStore();
    private readonly SmtpMailer _mailer = new SmtpMailer();

    public decimal Place(LegacyBill bill, string customerType)
    {
        decimal total = 0m;
        foreach (var line in bill.Lines)
            total += line.Tariff * line.Quantity;
        if (customerType == "regular")
            total = total * 0.95m;
        else if (customerType == "privileged")
            total = total * 0.90m;
        else if (customerType == "employee")
            total = total * 0.70m;
        if (total > 1000m)
            total = total - 50m;
        bill.Total = total;
        bill.Status = 1;
        _store.SaveToFile(bill, "bills.json");
        _mailer.Send(bill.Email, "Bill " + bill.Id, " " + total);
        return total;
    }

    public string BuildCsvReport(List<LegacyBill> bills)
    {
        var text = "id;total;status" + Environment.NewLine;
        foreach (var b in bills)
            text += b.Id + ";" + b.Total + ";" + b.Status;
        File.WriteAllText("report.csv", text);
        Console.WriteLine("Звіт збережено");
        return text;
    }

    public void Archive(LegacyBill bill)
    {
        LegacyBill archived = new LegacyArchivedBill(bill.Id);
        foreach (var line in bill.Lines)
            archived.AddLine(line);
        _store.SaveToFile(archived, "archive.json");
    }
}
