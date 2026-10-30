namespace UtilityBills.Legacy;

public sealed class LoyalDiscountRule : IDiscountRule
{
    public decimal? RateFor(Consumer client) =>
        client.IsLoyal ? PricingRules.LoyalRate : null;
}
