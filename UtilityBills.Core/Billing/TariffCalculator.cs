namespace UtilityBills.Core.Billing;

/// <summary>Профільне правило варіанта 14: сходинки тарифу та пільга.</summary>
public static class TariffCalculator
{
    /// <summary>Верхня межа першої сходинки, одиниць.</summary>
    public const decimal FirstStepLimit = 100m;

    /// <summary>Верхня межа другої сходинки, одиниць.</summary>
    public const decimal SecondStepLimit = 600m;

    /// <summary>Тариф першої сходинки, грн за одиницю.</summary>
    public const decimal FirstStepTariff = 2.64m;

    /// <summary>Тариф другої сходинки, грн за одиницю.</summary>
    public const decimal SecondStepTariff = 3.30m;

    /// <summary>Тариф третьої сходинки, грн за одиницю.</summary>
    public const decimal ThirdStepTariff = 4.20m;

    /// <summary>Частка тарифу першої сходинки, яку сплачує пільговик.</summary>
    public const decimal PrivilegeFactor = 0.5m;

    /// <summary>Розкладає споживання на сходинки тарифу.</summary>
    /// <param name="consumption">Споживання за період, не менше нуля.</param>
    /// <param name="isPrivileged">Чи має абонент пільгу.</param>
    /// <returns>Непорожні сходинки від першої до третьої.</returns>
    public static IReadOnlyList<TariffStepCharge> Steps(decimal consumption, bool isPrivileged)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(consumption);
        decimal firstQuantity = Math.Min(consumption, FirstStepLimit);
        decimal secondQuantity = Math.Min(
            Math.Max(consumption - FirstStepLimit, 0m),
            SecondStepLimit - FirstStepLimit);
        decimal thirdQuantity = Math.Max(consumption - SecondStepLimit, 0m);
        decimal firstTariff = isPrivileged
            ? FirstStepTariff * PrivilegeFactor
            : FirstStepTariff;
        List<TariffStepCharge> steps = new();
        AddIfPositive(steps, firstQuantity, firstTariff);
        AddIfPositive(steps, secondQuantity, SecondStepTariff);
        AddIfPositive(steps, thirdQuantity, ThirdStepTariff);
        return steps;
    }

    /// <summary>Обчислює нарахування: сума сходинок, кожна округлена до 0,01 грн угору.</summary>
    /// <param name="consumption">Споживання за період, не менше нуля.</param>
    /// <param name="isPrivileged">Чи має абонент пільгу.</param>
    /// <returns>Сума нарахування, грн.</returns>
    public static decimal ChargeFor(decimal consumption, bool isPrivileged)
    {
        decimal total = 0m;
        foreach (TariffStepCharge step in Steps(consumption, isPrivileged))
        {
            total += step.Amount;
        }

        return total;
    }

    private static void AddIfPositive(
        List<TariffStepCharge> steps,
        decimal quantity,
        decimal tariff)
    {
        if (quantity > 0m)
        {
            steps.Add(new TariffStepCharge(quantity, tariff));
        }
    }
}
