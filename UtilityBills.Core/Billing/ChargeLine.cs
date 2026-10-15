namespace UtilityBills.Core.Billing;

/// <summary>Рядок нарахування: послуга, обсяг і тариф.</summary>
public sealed class ChargeLine
{
    /// <inheritdoc/>
    public string ServiceCode { get; set; } = string.Empty;

    /// <inheritdoc/>
    public int Quantity { get; set; }

    /// <inheritdoc/>
    public decimal Tariff { get; set; }
}
