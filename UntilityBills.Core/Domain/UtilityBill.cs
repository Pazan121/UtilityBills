namespace UtilityBills.Core.Domain;

/// <summary>Рахунок за комунальні послуги для абонента.</summary>
public sealed class UtilityBill
{
    public int Id { get; init; }

    public int SubscriberId { get; init; }

    public string BillingPeriod { get; init; } = string.Empty;

    public BillStatus Status { get; set; } = BillStatus.Draft;

    public IReadOnlyList<UtilityBillLine> Lines { get; init; }

        = Array.Empty<UtilityBillLine>();

    public decimal Total => Lines.Sum(line => line.Amount);
}
