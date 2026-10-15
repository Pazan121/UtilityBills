namespace UtilityBills.Core.Domain;

/// <summary>Стан рахунку за комунальні послуги.</summary>
public enum BillStatus
{
    /// <inheritdoc/>
    Draft = 0,

    /// <inheritdoc/>
    Issued = 1,

    /// <inheritdoc/>
    Paid = 2,

    /// <inheritdoc/>
    Overdue = 3,

    /// <inheritdoc/>
    Cancelled = 4,
}
