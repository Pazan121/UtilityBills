namespace UtilityBills.Legacy;

public sealed class VipDiscountRule : IDiscountRule
{
    public decimal? RateFor(Consumer client) =>
        client.Kind == ConsumerKind.Vip ? PricingRules.VipRate : null;
}
