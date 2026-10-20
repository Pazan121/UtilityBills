using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Pricing;

/// <summary>Базова політика без знижок.</summary>
public sealed class StandardPricingPolicy : IPricingPolicy
{
    /// <inheritdoc/>
    public decimal PriceOf(UtilityBillLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount, 2);
    }
}
