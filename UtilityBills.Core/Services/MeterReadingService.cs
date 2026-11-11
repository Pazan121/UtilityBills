using Microsoft.Extensions.Logging;
using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;

namespace UtilityBills.Core.Services;

/// <summary>Сценарій UC-01: внести показник лічильника.</summary>
public sealed class MeterReadingService
{
    private readonly IMeterReadingRepository _readings;
    private readonly IReferenceData _reference;
    private readonly IClock _clock;
    private readonly ILogger<MeterReadingService> _log;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeterReadingService"/> class.
    /// </summary>
    /// <param name="readings">Сховище показників.</param>
    /// <param name="reference">Довідники абонентів і послуг.</param>
    /// <param name="clock">Джерело поточної дати.</param>
    /// <param name="log">Журнал подій.</param>
    public MeterReadingService(
        IMeterReadingRepository readings,
        IReferenceData reference,
        IClock clock,
        ILogger<MeterReadingService> log)
    {
        ArgumentNullException.ThrowIfNull(readings);
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(log);
        _readings = readings;
        _reference = reference;
        _clock = clock;
        _log = log;
    }

    /// <summary>Вносить показник; порушення правил — <see cref="DomainRuleException"/>.</summary>
    /// <param name="subscriberId">Ідентифікатор абонента.</param>
    /// <param name="utilityServiceId">Ідентифікатор послуги.</param>
    /// <param name="readingDate">Дата зняття показника, не пізніша за сьогодні.</param>
    /// <param name="value">Показник, не менший за попередній.</param>
    /// <returns>Збережений показник.</returns>
    public MeterReading Submit(
        int subscriberId,
        int utilityServiceId,
        DateOnly readingDate,
        decimal value)
    {
        using IDisposable? scope = _log.BeginScope("UC-01");
        _log.LogInformation("Внесення показника абонента {SubscriberId}", subscriberId);
        try
        {
            EnsureKnown(subscriberId, utilityServiceId);
            EnsureDateAllowed(readingDate);
            EnsureNotLessThanPrevious(subscriberId, utilityServiceId, readingDate, value);
            MeterReading reading = new(subscriberId, utilityServiceId, readingDate, value);
            _readings.Add(reading);
            _log.LogInformation("Показник збережено: {Value}", value);
            return reading;
        }
        catch (DomainRuleException ex)
        {
            _log.LogWarning("Показник відхилено, правило {Rule}", ex.Rule);
            throw;
        }
    }

    private void EnsureKnown(int subscriberId, int utilityServiceId)
    {
        if (_reference.FindSubscriber(subscriberId) is null)
        {
            throw new DomainRuleException(
                "subscriber.exists",
                $"Абонента {subscriberId} не знайдено");
        }

        if (_reference.FindService(utilityServiceId) is null)
        {
            throw new DomainRuleException(
                "service.exists",
                $"Послугу {utilityServiceId} не знайдено");
        }
    }

    private void EnsureDateAllowed(DateOnly readingDate)
    {
        if (readingDate > _clock.Today)
        {
            throw new DomainRuleException(
                "reading.future",
                "Дата показника не може бути в майбутньому");
        }
    }

    private void EnsureNotLessThanPrevious(
        int subscriberId,
        int utilityServiceId,
        DateOnly readingDate,
        decimal value)
    {
        IReadOnlyList<MeterReading> history = _readings.GetFor(subscriberId, utilityServiceId);
        if (history.Count == 0)
        {
            return;
        }

        MeterReading last = history[^1];
        if (readingDate <= last.ReadingDate)
        {
            throw new DomainRuleException(
                "reading.date",
                $"Дата має бути пізнішою за {last.ReadingDate:yyyy-MM-dd}");
        }

        if (value < last.Value)
        {
            throw new DomainRuleException(
                "meter.rollback",
                $"Показник {value} менший за попередній {last.Value}");
        }
    }
}
