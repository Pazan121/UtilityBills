namespace UtilityBills.Core.Domain;

/// <summary>Рахунок за комунальні послуги для абонента.</summary>
public sealed class UtilityBill
{
    /// <inheritdoc/>
    public int Id { get; init; }

    /// <inheritdoc/>
    public int SubscriberId { get; init; }

    /// <inheritdoc/>
    public string BillingPeriod { get; init; } = string.Empty;

    /// <inheritdoc/>
    public BillStatus Status { get; set; } = BillStatus.Draft;

    /// <inheritdoc/>
    public IReadOnlyList<UtilityBillLine> Lines { get; init; }

        = Array.Empty<UtilityBillLine>();

    /// <inheritdoc/>
    public decimal Total => Lines.Sum(line => line.Amount);
}
