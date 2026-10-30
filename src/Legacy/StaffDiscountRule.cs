namespace UtilityBills.Legacy;

public sealed class StaffDiscountRule : IDiscountRule
{
    public decimal? RateFor(Consumer client) =>
        client.Kind == ConsumerKind.Staff ? PricingRules.StaffRate : null;
}
