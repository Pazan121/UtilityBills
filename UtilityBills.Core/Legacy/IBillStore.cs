#pragma warning disable CS1591, IDE0011, SA1503, SA1402, SA1649, CA1002, CA1822, CA1051
namespace UtilityBills.Core.Legacy;

/// <summary>Навмисно широкий інтерфейс сховища (приклад порушення ISP).</summary>
public interface IBillStore
{
    void Add(LegacyBill bill);

    LegacyBill GetById(int id);

    void SaveToFile(LegacyBill bill, string path);

    void SendEmail(string recipient, string body);

    string ExportCsv();

    void Backup(string folder);

    void PrintInvoice(LegacyBill bill);
}
