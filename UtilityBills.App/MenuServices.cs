using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Services;

namespace UtilityBills.App;

internal sealed class MenuServices
{
    public required MeterReadingService Meters { get; init; }

    public required ChargeService Charges { get; init; }

    public required BillService Bills { get; init; }

    public required BillSearchService Search { get; init; }

    public required IReferenceData Reference { get; init; }

    public required IClock Clock { get; init; }

    public required Action Save { get; init; }
}
