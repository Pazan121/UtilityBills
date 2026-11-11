using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Abstractions;

/// <summary>Довідники абонентів і послуг.</summary>
public interface IReferenceData
{
    /// <summary>Шукає абонента за ідентифікатором.</summary>
    /// <param name="id">Ідентифікатор абонента.</param>
    /// <returns>Абонент або null.</returns>
    Subscriber? FindSubscriber(int id);

    /// <summary>Шукає послугу за ідентифікатором.</summary>
    /// <param name="id">Ідентифікатор послуги.</param>
    /// <returns>Послуга або null.</returns>
    UtilityService? FindService(int id);

    /// <summary>Повертає всіх абонентів.</summary>
    /// <returns>Список абонентів.</returns>
    IReadOnlyList<Subscriber> GetSubscribers();
}
