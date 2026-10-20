#pragma warning disable CS1591, IDE0011, SA1503, SA1402, SA1649, CA1002, CA1822, CA1051
namespace UtilityBills.Core.Legacy;

/// <summary>Спрощена файлова реалізація зі старого проєкту.</summary>
public class FileBillStore
{
    public void SaveToFile(LegacyBill bill, string path)
    {
        File.WriteAllText(path, bill.Id + ";" + bill.Total);
    }
}
