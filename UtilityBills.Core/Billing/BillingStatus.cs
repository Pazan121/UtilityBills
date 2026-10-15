namespace UtilityBills.Core.Billing;

/// <summary>
/// Визначає поточний статус рахунку (біллінгу).
/// </summary>
public enum BillingStatus
{
    /// <summary>
    /// Рахунок виставлено для оплати.
    /// </summary>
    Issued,

    /// <summary>
    /// Рахунок успішно оплачено.
    /// </summary>
    Paid,

    /// <summary>
    /// Рахунок закрито.
    /// </summary>
    Closed,

    /// <summary>
    /// Рахунок скасовано.
    /// </summary>
    Cancelled
}
