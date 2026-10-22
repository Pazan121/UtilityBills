using System.Globalization;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;

namespace UtilityBills.App;

/// <summary>Барикада: перетворює введені рядки на типізований рядок рахунку.</summary>
public static class BillLineParser
{
    /// <summary>Розбирає введення; роздільник дробової частини — за культурою ОС.</summary>
    /// <param name="serviceId">Код послуги.</param>
    /// <param name="quantity">Обсяг послуги.</param>
    /// <param name="tariff">Тариф за одиницю.</param>
    /// <returns>Готовий рядок рахунку або пояснення помилки.</returns>
    public static Result<UtilityBillLine> Parse(
        string? serviceId,
        string? quantity,
        string? tariff)
    {
        CultureInfo culture = CultureInfo.CurrentCulture;
        if (!int.TryParse(serviceId, NumberStyles.Integer, culture, out int id) || id <= 0)
        {
            return Result<UtilityBillLine>.Fail("Код послуги — ціле число більше 0");
        }

        if (!decimal.TryParse(quantity, NumberStyles.Number, culture, out decimal volume)
            || volume <= 0m)
        {
            return Result<UtilityBillLine>.Fail("Обсяг — число більше 0");
        }

        if (!decimal.TryParse(tariff, NumberStyles.Number, culture, out decimal price)
            || price < 0m)
        {
            return Result<UtilityBillLine>.Fail("Тариф не може бути від’ємним");
        }

        return Result<UtilityBillLine>.Ok(new UtilityBillLine(id, volume, price));
    }
}
