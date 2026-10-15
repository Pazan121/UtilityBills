namespace UtilityBills.Core.Domain;

/// <summary>Рядок рахунку з обсягом послуги та тарифом.</summary>
public sealed class UtilityBillLine
{
    /// <inheritdoc/>
    public int Id { get; init; }

    /// <inheritdoc/>
    public int UtilityServiceId { get; init; }

    /// <inheritdoc/>
    public decimal Quantity { get; init; }

    /// <inheritdoc/>
    public decimal Tariff { get; init; }

    /// <inheritdoc/>
    public decimal Amount => Quantity * Tariff;
}
