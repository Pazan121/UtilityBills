using System.Globalization;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Parsing;

/// <summary>Межа між «брудними» даними ззовні та сутностями.</summary>
public static class UtilityBillLineParser
{
    /// <summary>Найбільший допустимий код послуги.</summary>
    public const int MaxServiceId = 9999;

    /// <summary>Найбільший допустимий обсяг послуги.</summary>
    public const decimal MaxQuantity = 100000m;

    /// <summary>Найбільший допустимий тариф.</summary>
    public const decimal MaxTariff = 100000m;

    /// <summary>
    /// Розбирає рядок формату «код;обсяг;тариф»; число з крапкою, незалежно від культури.
    /// </summary>
    /// <param name="raw">Вхідний рядок.</param>
    /// <param name="line">Готовий рядок рахунку або null.</param>
    /// <returns>true, якщо рядок коректний.</returns>
    public static bool TryParse(string? raw, out UtilityBillLine? line)
    {
        line = null;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var parts = raw.Split(';');
        if (parts.Length != 3)
        {
            return false;
        }

        var culture = CultureInfo.InvariantCulture;
        const NumberStyles integer = NumberStyles.AllowLeadingSign;
        const NumberStyles number =
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        if (!int.TryParse(parts[0].Trim(), integer, culture, out var serviceId)
            || serviceId is < 1 or > MaxServiceId)
        {
            return false;
        }

        if (!decimal.TryParse(parts[1].Trim(), number, culture, out var quantity)
            || quantity is <= 0m or > MaxQuantity)
        {
            return false;
        }

        if (!decimal.TryParse(parts[2].Trim(), number, culture, out var tariff)
            || tariff is < 0m or > MaxTariff)
        {
            return false;
        }

        line = new UtilityBillLine(serviceId, quantity, tariff);
        return true;
    }
}
