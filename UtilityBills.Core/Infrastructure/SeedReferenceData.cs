using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Infrastructure;

/// <summary>Умовні довідники: 60 абонентів у 12 будинках та одна послуга.</summary>
public sealed class SeedReferenceData : IReferenceData
{
    private const int _subscriberCount = 60;
    private const int _subscribersPerHouse = 5;
    private const int _privilegedEvery = 7;

    private readonly List<Subscriber> _subscribers = new();
    private readonly UtilityService _electricity = new(1, "Електроенергія", 2.64m);

    /// <summary>Initializes a new instance of the <see cref="SeedReferenceData"/> class.</summary>
    public SeedReferenceData()
    {
        for (int number = 1; number <= _subscriberCount; number++)
        {
            int house = ((number - 1) / _subscribersPerHouse) + 1;
            _subscribers.Add(new Subscriber(
                number,
                $"Абонент {number:000}",
                $"Київ, вул. Вишнева, {house}",
                $"abonent{number}@example.com",
                number % _privilegedEvery == 0));
        }
    }

    /// <inheritdoc/>
    public Subscriber? FindSubscriber(int id)
    {
        return _subscribers.Find(subscriber => subscriber.Id == id);
    }

    /// <inheritdoc/>
    public UtilityService? FindService(int id)
    {
        return id == _electricity.Id ? _electricity : null;
    }

    /// <inheritdoc/>
    public IReadOnlyList<Subscriber> GetSubscribers()
    {
        return _subscribers;
    }
}
