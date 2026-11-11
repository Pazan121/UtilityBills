using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Tests;

internal sealed class FixedClock : IClock
{
    public FixedClock(DateOnly today)
    {
        Today = today;
    }

    public DateOnly Today { get; }
}

internal sealed class FakeReferenceData : IReferenceData
{
    private readonly List<Subscriber> _subscribers = new()
    {
        new Subscriber(1, "Абонент 001", "Київ, вул. Вишнева, 1", "a1@example.com"),
        new Subscriber(2, "Абонент 002", "Київ, вул. Вишнева, 1", "a2@example.com"),
        new Subscriber(7, "Абонент 007", "Київ, вул. Вишнева, 2", "a7@example.com", true),
    };

    public Subscriber? FindSubscriber(int id)
    {
        return _subscribers.Find(subscriber => subscriber.Id == id);
    }

    public UtilityService? FindService(int id)
    {
        return id == 1 ? new UtilityService(1, "Електроенергія", 2.64m) : null;
    }

    public IReadOnlyList<Subscriber> GetSubscribers()
    {
        return _subscribers;
    }
}

internal sealed class FakeMeterReadings : IMeterReadingRepository
{
    private readonly List<MeterReading> _items = new();

    public void Add(MeterReading reading)
    {
        _items.Add(reading);
    }

    public IReadOnlyList<MeterReading> GetFor(int subscriberId, int utilityServiceId)
    {
        return _items
            .Where(item => item.SubscriberId == subscriberId
                && item.UtilityServiceId == utilityServiceId)
            .OrderBy(item => item.ReadingDate)
            .ToList();
    }
}

internal sealed class SpyNotifier : INotifier
{
    public List<string> Messages { get; } = new();

    public void Notify(string recipient, string message)
    {
        Messages.Add(recipient + ": " + message);
    }
}
