using System.Globalization;
using System.Text;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Reporting;

/// <summary>Сценарій UC-06: звіт «нарахування за будинком».</summary>
public static class HouseChargeReport
{
    /// <summary>Будує текст звіту; формат чисел не залежить від культури системи.</summary>
    /// <param name="address">Адреса будинку.</param>
    /// <param name="subscribers">Усі абоненти.</param>
    /// <param name="bills">Усі рахунки.</param>
    /// <param name="totalOf">Функція обчислення суми рахунку.</param>
    /// <returns>Текст звіту.</returns>
    public static string Build(
        string address,
        IReadOnlyList<Subscriber> subscribers,
        IReadOnlyList<UtilityBill> bills,
        Func<UtilityBill, decimal> totalOf)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        ArgumentNullException.ThrowIfNull(subscribers);
        ArgumentNullException.ThrowIfNull(bills);
        ArgumentNullException.ThrowIfNull(totalOf);
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine(culture, $"Нарахування за будинком: {address}");
        decimal grandTotal = 0m;
        int billCount = 0;
        IEnumerable<Subscriber> inHouse = subscribers
            .Where(candidate => candidate.Address == address)
            .OrderBy(candidate => candidate.FullName, StringComparer.Ordinal);
        foreach (Subscriber subscriber in inHouse)
        {
            List<UtilityBill> own = bills
                .Where(bill => bill.SubscriberId == subscriber.Id)
                .ToList();
            if (own.Count == 0)
            {
                continue;
            }

            decimal sum = own.Sum(totalOf);
            text.AppendLine(culture, $"{subscriber.FullName}: {sum:F2}");
            grandTotal += sum;
            billCount += own.Count;
        }

        if (billCount == 0)
        {
            text.AppendLine("Рахунків немає.");
        }
        else
        {
            text.AppendLine(culture, $"Разом: {grandTotal:F2} ({billCount} рах.)");
        }

        return text.ToString();
    }
}
