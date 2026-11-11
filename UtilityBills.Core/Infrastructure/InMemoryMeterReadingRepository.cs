using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Infrastructure;

/// <summary>Сховище показників у пам'яті.</summary>
public class InMemoryMeterReadingRepository : IMeterReadingRepository
{
    private readonly List<MeterReading> _items = new();

    /// <summary>Gets the всі показники, потрібні для збереження у файл.</summary>
    protected IReadOnlyList<MeterReading> Items => _items;

    /// <inheritdoc/>
    public void Add(MeterReading reading)
    {
        ArgumentNullException.ThrowIfNull(reading);
        _items.Add(reading);
    }

    /// <inheritdoc/>
    public IReadOnlyList<MeterReading> GetFor(int subscriberId, int utilityServiceId)
    {
        return _items
            .Where(item => item.SubscriberId == subscriberId
                && item.UtilityServiceId == utilityServiceId)
            .OrderBy(item => item.ReadingDate)
            .ToList();
    }
}
