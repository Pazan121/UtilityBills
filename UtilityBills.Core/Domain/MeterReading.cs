namespace UtilityBills.Core.Domain;

/// <summary>Показник лічильника: незмінне значення на дату.</summary>
public sealed record MeterReading
{
    /// <summary>Initializes a new instance of the <see cref="MeterReading"/> class.</summary>
    /// <param name="subscriberId">Ідентифікатор абонента, більший за нуль.</param>
    /// <param name="utilityServiceId">Ідентифікатор послуги, більший за нуль.</param>
    /// <param name="readingDate">Дата зняття показника.</param>
    /// <param name="value">Показник лічильника, не менший за нуль.</param>
    public MeterReading(
        int subscriberId,
        int utilityServiceId,
        DateOnly readingDate,
        decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(subscriberId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(utilityServiceId);
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        SubscriberId = subscriberId;
        UtilityServiceId = utilityServiceId;
        ReadingDate = readingDate;
        Value = value;
    }

    /// <summary>Gets the ідентифікатор абонента.</summary>
    public int SubscriberId { get; }

    /// <summary>Gets the ідентифікатор послуги.</summary>
    public int UtilityServiceId { get; }

    /// <summary>Gets the дату зняття показника.</summary>
    public DateOnly ReadingDate { get; }

    /// <summary>Gets the значення показника.</summary>
    public decimal Value { get; }
}
