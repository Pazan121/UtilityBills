namespace UtilityBills.Core.Domain;

/// <summary>Комунальна послуга та її тариф.</summary>
public sealed class UtilityService
{
    /// <inheritdoc/>
    public int Id { get; init; }

    /// <inheritdoc/>
    public string Name { get; init; } = string.Empty;

    /// <inheritdoc/>
    public decimal Tariff { get; init; }
}
