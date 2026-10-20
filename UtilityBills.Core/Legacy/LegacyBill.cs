#pragma warning disable CS1591, IDE0011, SA1503, SA1402, SA1649, CA1002, CA1822, CA1051
namespace UtilityBills.Core.Legacy;

/// <summary>Застарілий рядок рахунку з публічними сетерами.</summary>
public class LegacyBillLine
{
    public string ServiceCode { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal Tariff { get; set; }
}

/// <summary>Застарілий рахунок: стан у вигляді числа, публічні сетери.</summary>
public class LegacyBill
{
    public int Id { get; set; }

    public int Status { get; set; }

    public decimal Total { get; set; }

    public string Email { get; set; } = string.Empty;

    public List<LegacyBillLine> Lines { get; set; } = new();

    public virtual void AddLine(LegacyBillLine line) =>
        Lines.Add(line);
}

/// <summary>Архівний рахунок як нащадок (приклад порушення LSP).</summary>
public class LegacyArchivedBill : LegacyBill
{
    public LegacyArchivedBill(int id) => Id = id;

    public override void AddLine(LegacyBillLine line)
    {
        throw new NotSupportedException("Архів змінювати не можна");
    }
}
