using UtilityBills.Core.Abstractions;

namespace UtilityBills.Core.Infrastructure;

/// <summary>Системний годинник.</summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc/>
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}
