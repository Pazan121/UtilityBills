using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Pricing;

/// <summary>Політика з постійним відсотком знижки.</summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private const decimal MaxRate = 0.5m;

    private readonly decimal _rate;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscountPricingPolicy"/> class.
    /// </summary>
    /// <param name="rate">Частка знижки від 0 до 0,5.</param>
    public DiscountPricingPolicy(decimal rate)
    {
        if (rate < 0m || rate > MaxRate)
        {
            throw new ArgumentOutOfRangeException(nameof(rate));
        }

        _rate = rate;
    }

    /// <inheritdoc/>
    public decimal PriceOf(UtilityBillLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * (1m - _rate), 2);
    }
}
