namespace UtilityBills.Core.Domain;

/// <summary>Абонент, якому нараховуються комунальні платежі.</summary>
public sealed class Subscriber
{
    /// <inheritdoc/>
    public int Id { get; init; }

    /// <inheritdoc/>
    public string FullName { get; init; } = string.Empty;

    /// <inheritdoc/>
    public string Address { get; init; } = string.Empty;
}
