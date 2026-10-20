namespace UtilityBills.Core.Domain;

/// <summary>Архівний рахунок: не є рахунком, а лише показує його дані.</summary>
public sealed class ArchivedBill
{
    private readonly UtilityBill _bill;

    /// <summary>Initializes a new instance of the <see cref="ArchivedBill"/> class.</summary>
    /// <param name="bill">Оплачений або скасований рахунок.</param>
    /// <param name="archivedAt">Дата переносу в архів.</param>
    public ArchivedBill(UtilityBill bill, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(bill);
        if (bill.Status != BillStatus.Paid && bill.Status != BillStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "В архів переносять лише оплачені або скасовані рахунки.");
        }

        _bill = bill;
        ArchivedAt = archivedAt;
    }

    /// <summary>Gets the дату переносу в архів.</summary>
    public DateOnly ArchivedAt { get; }

    /// <summary>Gets the ідентифікатор рахунку.</summary>
    public int Id => _bill.Id;

    /// <summary>Gets the стан рахунку на момент архівування.</summary>
    public BillStatus Status => _bill.Status;

    /// <summary>Gets the рядки рахунку лише для читання.</summary>
    public IReadOnlyList<UtilityBillLine> Lines => _bill.Lines;
}
