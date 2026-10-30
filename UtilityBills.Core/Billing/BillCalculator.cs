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

    /// <summary>Обчислює суму до сплати за сумою за тарифом і ознакою пільги.</summary>
    /// <param name="amount">Сума за тарифом до знижок, грн.</param>
    /// <param name="isPrivileged">Чи має споживач пільгу.</param>
    /// <returns>Сума до сплати, грн.</returns>
    public decimal ChargeFor(decimal amount, bool isPrivileged)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Сума не може бути від’ємною.");
        }

        decimal rate = 0m;
        if (isPrivileged)
        {
            rate += PrivilegedRate;
        }

        if (amount >= BigBillFrom)
        {
            rate += BigBillRate;
        }

        return decimal.Ceiling(amount * (1m - rate) * 100m) / 100m;
    }
}
