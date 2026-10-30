namespace UtilityBills.Legacy;

public sealed class DiscountPolicy
{
    public decimal For(decimal subtotal, ConsumerKind kind, int doneCount)
    {
        decimal rate = RateFor(kind, doneCount);
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private static decimal RateFor(ConsumerKind kind, int doneCount) => kind switch
    {
        ConsumerKind.Vip => PricingRules.VipRate,
        ConsumerKind.Staff => PricingRules.StaffRate,
        _ => doneCount > PricingRules.LoyalOrders ? PricingRules.LoyalRate : 0m,
    };
}
