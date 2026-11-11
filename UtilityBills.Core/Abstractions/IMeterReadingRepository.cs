using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Abstractions;

/// <summary>Сховище показників лічильників.</summary>
public interface IMeterReadingRepository
{
    /// <summary>Додає показник.</summary>
    /// <param name="reading">Показник лічильника.</param>
    void Add(MeterReading reading);

    /// <summary>Повертає показники абонента за послугою від найстарішого до найновішого.</summary>
    /// <param name="subscriberId">Ідентифікатор абонента.</param>
    /// <param name="utilityServiceId">Ідентифікатор послуги.</param>
    /// <returns>Копія списку показників, упорядкована за датою.</returns>
    IReadOnlyList<MeterReading> GetFor(int subscriberId, int utilityServiceId);
}
