namespace UtilityBills.Core.Domain;

/// <summary>Рядок рахунку: незмінна позиція документа.</summary>
public sealed record UtilityBillLine
{
    /// <summary>Initializes a new instance of the <see cref="UtilityBillLine"/> class.</summary>
    /// <param name="utilityServiceId">Ідентифікатор послуги, більший за нуль.</param>
    /// <param name="quantity">Обсяг послуги, більший за нуль.</param>
    /// <param name="tariff">Тариф, не менший за нуль.</param>
    public UtilityBillLine(int utilityServiceId, decimal quantity, decimal tariff)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(utilityServiceId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(tariff);
        UtilityServiceId = utilityServiceId;
        Quantity = quantity;
        Tariff = tariff;
    }

    /// <summary>Gets the ідентифікатор послуги.</summary>
    public int UtilityServiceId { get; }

    /// <summary>Gets the обсяг послуги.</summary>
    public decimal Quantity { get; }

    /// <summary>Gets the тариф за одиницю обсягу.</summary>
    public decimal Tariff { get; }

    /// <summary>Gets the суму рядка; обчислюється, а не зберігається.</summary>
    public decimal Amount => Quantity * Tariff;
}
