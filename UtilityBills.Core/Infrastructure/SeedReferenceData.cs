using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Infrastructure;

/// <summary>Умовні довідники: 60 абонентів у 12 будинках та одна послуга.</summary>
public sealed class SeedReferenceData : IReferenceData
{
    private const int SubscriberCount = 60;
    private const int SubscribersPerHouse = 5;
    private const int PrivilegedEvery = 7;

    private readonly List<Subscriber> _subscribers = new();
    private readonly UtilityService _electricity = new(1, "Електроенергія", 2.64m);

    /// <summary>Initializes a new instance of the <see cref="SeedReferenceData"/> class.</summary>
    public SeedReferenceData()
    {
        for (int number = 1; number <= SubscriberCount; number++)
        {
            int house = ((number - 1) / SubscribersPerHouse) + 1;
            _subscribers.Add(new Subscriber(
                number,
                $"Абонент {number:000}",
                $"Київ, вул. Вишнева, {house}",
                $"abonent{number}@example.com",
                number % PrivilegedEvery == 0));
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
