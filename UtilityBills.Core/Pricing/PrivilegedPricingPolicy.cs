using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Pricing;

/// <summary>Політика для пільгових абонентів: знижка 10 %.</summary>
public sealed class PrivilegedPricingPolicy : IPricingPolicy
{
    private const decimal _factor = 0.90m;

    /// <inheritdoc/>
    public decimal PriceOf(UtilityBillLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * _factor, 2);
    }
}
