using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Infrastructure;

internal sealed record BillDto(
    int Id,
    int SubscriberId,
    string BillingPeriod,
    DateOnly DueDate,
    BillStatus Status,
    List<LineDto> Lines);
