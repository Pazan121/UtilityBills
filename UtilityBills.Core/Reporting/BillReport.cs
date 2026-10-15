using System.Globalization;
using System.Text;
using UtilityBills.Core.Billing;

namespace UtilityBills.Core.Reporting;

/// <summary>Формування тексту звіту про рахунок (чиста функція).</summary>
public static class BillReport
{
    /// <summary>Будує текст звіту; результат не залежить від культури ОС.</summary>
    /// <param name="request">Запит на розрахунок.</param>
    /// <param name="result">Результат розрахунку.</param>
    /// <returns>Текст звіту.</returns>
    public static string TextOf(
        BillTotalRequest request,
        BillTotalResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine(culture, $"Рахунок № {request.BillId}");
        text.AppendLine(culture, $"Абонент: {request.Subscriber.Name}");
        foreach (ChargeLine line in request.Lines)
        {
            text.AppendLine(culture, $"{line.ServiceCode} x {line.Quantity}");
        }

        text.AppendLine(culture, $"Разом: {result.Total:F2}");
        text.AppendLine(culture, $"Стан: {result.Status}");
        return text.ToString();
    }
}
