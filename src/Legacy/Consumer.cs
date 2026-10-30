namespace UtilityBills.Legacy;

public sealed class Consumer
{
    public int Id { get; init; }

    public string Name { get; init; } = "";

    public string Email { get; init; } = "";

    public ConsumerKind Kind { get; init; }

    public int DoneCount { get; init; }

    public DateTime SinceUtc { get; init; }

    public bool HasValidEmail =>
        Email is not null && Email.Contains("@");

    public bool IsLoyal =>
        DoneCount > PricingRules.LoyalOrders;
}
