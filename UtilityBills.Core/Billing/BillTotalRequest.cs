namespace UtilityBills.Core.Billing;

/// <summary>Запит на розрахунок підсумку рахунку.</summary>
public sealed record BillTotalRequest(
    int BillId,
    SubscriberInfo Subscriber,
    IReadOnlyList<ChargeLine> Lines,
    DateOnly CreatedAt,
    string? CouponCode,
    BillingStatus Status,
    decimal DeliveryPrice);
