namespace UtilityBills.Core.Billing;

/// <summary>Перевірки передумов для даних розрахунку нарахування.</summary>
public static class BillGuards
{
    /// <summary>Перевіряє номер рахунку та дані абонента.</summary>
    /// <param name="billId">Номер рахунку.</param>
    /// <param name="subscriberName">Ім'я абонента.</param>
    /// <param name="subscriberEmail">Пошта абонента.</param>
    public static void EnsureHeaderValid(
        int billId,
        string subscriberName,
        string subscriberEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(billId);
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriberName);
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriberEmail);
        if (!subscriberEmail.Contains('@'))
        {
            throw new ArgumentException(
                "Пошта має містити символ @.",
                nameof(subscriberEmail));
        }
    }

    /// <summary>Перевіряє один рядок нарахування.</summary>
    /// <param name="line">Рядок нарахування.</param>
    public static void EnsureLineValid(ChargeLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.ServiceCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
            line.Quantity,
            nameof(line.Quantity));
        ArgumentOutOfRangeException.ThrowIfNegative(
            line.Tariff,
            nameof(line.Tariff));
    }

    /// <summary>Перевіряє колекцію рядків нарахування.</summary>
    /// <param name="lines">Рядки нарахування.</param>
    public static void EnsureLinesValid(IReadOnlyList<ChargeLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Count == 0)
        {
            throw new ArgumentException(
                "Рахунок не містить жодної позиції.",
                nameof(lines));
        }

        foreach (ChargeLine line in lines)
        {
            EnsureLineValid(line);
        }
    }
}
