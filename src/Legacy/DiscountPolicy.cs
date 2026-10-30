namespace UtilityBills.Legacy;

public sealed class DiscountPolicy
{
    private readonly IReadOnlyList<IDiscountRule> _rules;

    public DiscountPolicy()
        : this(new IDiscountRule[]
        {
            new VipDiscountRule(),
            new StaffDiscountRule(),
            new LoyalDiscountRule(),
        })
    {
    }

    public DiscountPolicy(IReadOnlyList<IDiscountRule> rules)
    {
        _rules = rules;
    }

    public decimal For(decimal subtotal, Consumer client)
    {
        return Math.Min(subtotal * RateFor(client), PricingRules.MaxDiscount);
    }

    private decimal RateFor(Consumer client)
    {
        foreach (var rule in _rules)
        {
            var rate = rule.RateFor(client);
            if (rate is not null) return rate.Value;
        }
        return 0m;
    }
}
