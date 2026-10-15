namespace UtilityBills.Core.Billing;

/// <summary>Дані абонента, потрібні для розрахунку.</summary>
public sealed record SubscriberInfo(
    string Name,
    string Email,
    bool IsRegular);
