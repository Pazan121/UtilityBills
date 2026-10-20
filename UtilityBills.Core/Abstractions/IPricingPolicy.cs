using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Abstractions;

/// <summary>Політика ціноутворення для одного рядка рахунку.</summary>
public interface IPricingPolicy
{
    /// <summary>Обчислює вартість рядка за правилом політики.</summary>
    /// <param name="line">Рядок рахунку.</param>
    /// <returns>Вартість рядка.</returns>
    decimal PriceOf(UtilityBillLine line);
}
