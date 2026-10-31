#pragma warning disable CA1822 // Екземпляр потрібен тестам і майбутнім залежностям.
namespace UtilityBills.Core.Billing;

/// <summary>Нарахування за тарифом: пільга, знижка за великий рахунок і округлення.</summary>
public sealed class BillCalculator
{
    /// <summary>Частка знижки пільговому споживачеві.</summary>
    public const decimal PrivilegedRate = 0.05m;

    /// <summary>Частка додаткової знижки за великий рахунок.</summary>
    public const decimal BigBillRate = 0.10m;

    /// <summary>Межа великого рахунку, грн.</summary>
    public const decimal BigBillFrom = 1000m;

    /// <summary>Кількість копійок у гривні.</summary>
    public const decimal CentsInHryvnia = 100m;

    /// <summary>Обчислює суму до сплати; результат округлено до 0,01 грн угору.</summary>
    /// <param name="amount">Сума за тарифом до знижок, грн.</param>
    /// <param name="isPrivileged">Чи має споживач пільгу.</param>
    /// <returns>Сума до сплати, грн.</returns>
    public decimal ChargeFor(decimal amount, bool isPrivileged)
    {
        EnsureAmountIsValid(amount);
        decimal raw = amount * (1m - RateFor(amount, isPrivileged));
        return RoundUpToCent(raw);
    }

    private static void EnsureAmountIsValid(decimal amount)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Сума не може бути від’ємною.");
        }
    }

    private static decimal RateFor(decimal amount, bool isPrivileged)
    {
        decimal rate = isPrivileged ? PrivilegedRate : 0m;
        if (amount >= BigBillFrom)
        {
            rate += BigBillRate;
        }

        return rate;
    }

    private static decimal RoundUpToCent(decimal value)
    {
        return decimal.Ceiling(value * CentsInHryvnia) / CentsInHryvnia;
    }
}
