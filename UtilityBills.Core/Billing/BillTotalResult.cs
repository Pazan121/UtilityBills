namespace UtilityBills.Core.Billing;

/// <summary>Результат розрахунку: підсумок і новий стан рахунку.</summary>
public sealed record BillTotalResult(
    decimal Total,
    BillingStatus Status);
