using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Pricing;

/// <summary>Політика нарахування за сходинками: рядок округлюється до 0,01 грн угору.</summary>
public sealed class CeilingPricingPolicy : IPricingPolicy
{
    private const decimal _centsInHryvnia = 100m;

    /// <inheritdoc/>
    public decimal PriceOf(UtilityBillLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return decimal.Ceiling(line.Amount * _centsInHryvnia) / _centsInHryvnia;
    }
}
