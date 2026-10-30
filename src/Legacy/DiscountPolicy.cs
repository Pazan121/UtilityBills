namespace UtilityBills.Legacy;

public sealed class DiscountPolicy
{
    public decimal For(decimal subtotal, Consumer client)
    {
        decimal rate = RateFor(client);
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private static decimal RateFor(Consumer client) => client.Kind switch
    {
        ConsumerKind.Vip => PricingRules.VipRate,
        ConsumerKind.Staff => PricingRules.StaffRate,
        _ => client.IsLoyal ? PricingRules.LoyalRate : 0m,
    };
}
