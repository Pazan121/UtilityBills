namespace UtilityBills.Core.Billing;

/// <summary>Розрахунок підсумку рахунку з набору коротких підпрограм.</summary>
public static class BillTotalCalculator
{
    /// <summary>Обчислює підсумок рахунку та його наступний стан.</summary>
    /// <param name="request">Запит на розрахунок.</param>
    /// <returns>Підсумок і новий стан рахунку.</returns>
    public static BillTotalResult Calculate(BillTotalRequest request)
    {
        BillGuards.EnsureRequestValid(request);
        decimal total = SubtotalOf(request.Lines);
        total -= ClientDiscountOf(total, request.Subscriber.IsRegular);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, request.CouponCode);
        total -= WeekendDiscountOf(total, request.CreatedAt);
        total += DeliveryFeeOf(total, request.DeliveryPrice);
        total = request.Status == BillingStatus.Cancelled
            ? 0m
            : Math.Round(Math.Max(total, 0m), 2);
        return new BillTotalResult(
            total,
            NextStatusOf(request.Status, total));
    }

    /// <summary>Сума рядків із гуртовою знижкою.</summary>
    /// <param name="lines">Рядки нарахування.</param>
    /// <returns>Підсумок рядків.</returns>
    public static decimal SubtotalOf(IReadOnlyList<ChargeLine> lines)
    {
        BillGuards.EnsureLinesValid(lines);
        decimal subtotal = 0m;
        foreach (ChargeLine line in lines)
        {
            decimal sum = line.Quantity * line.Tariff;
            if (line.Quantity >= PricingRules.BulkQuantity)
            {
                sum -= sum * PricingRules.BulkRate;
            }

            subtotal += sum;
        }

        return subtotal;
    }

    /// <summary>Знижка постійного абонента.</summary>
    /// <param name="amount">Поточна сума.</param>
    /// <param name="isRegularClient">Чи є абонент постійним.</param>
    /// <returns>Розмір знижки.</returns>
    public static decimal ClientDiscountOf(
        decimal amount,
        bool isRegularClient)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return isRegularClient
            ? amount * PricingRules.RegularClientRate
            : 0m;
    }

    /// <summary>Знижка за обсягом нарахування.</summary>
    /// <param name="amount">Поточна сума.</param>
    /// <returns>Розмір знижки.</returns>
    public static decimal VolumeDiscountOf(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        if (amount > PricingRules.LargeOrderFrom)
        {
            return amount * PricingRules.LargeOrderRate;
        }

        if (amount > PricingRules.MediumOrderFrom)
        {
            return amount * PricingRules.MediumOrderRate;
        }

        return 0m;
    }

    /// <summary>Знижка за кодом купона.</summary>
    /// <param name="amount">Поточна сума.</param>
    /// <param name="couponCode">Код купона або null.</param>
    /// <returns>Розмір знижки.</returns>
    public static decimal CouponDiscountOf(
        decimal amount,
        string? couponCode)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return couponCode switch
        {
            PricingRules.SaleCoupon
                => amount * PricingRules.CouponSaleRate,
            PricingRules.FixedCoupon
                when amount > PricingRules.CouponFixedFrom
                => PricingRules.CouponFixed,
            _ => 0m,
        };
    }

    /// <summary>Знижка за створення рахунку в неділю.</summary>
    /// <param name="amount">Поточна сума.</param>
    /// <param name="createdAt">Дата створення рахунку.</param>
    /// <returns>Розмір знижки.</returns>
    public static decimal WeekendDiscountOf(
        decimal amount,
        DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday
            ? amount * PricingRules.WeekendRate
            : 0m;
    }

    /// <summary>Плата за доставку рахунку.</summary>
    /// <param name="amount">Поточна сума.</param>
    /// <param name="deliveryPrice">Вартість доставки.</param>
    /// <returns>Плата за доставку.</returns>
    public static decimal DeliveryFeeOf(
        decimal amount,
        decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(deliveryPrice);
        return amount < PricingRules.FreeDeliveryFrom
            ? deliveryPrice
            : 0m;
    }

    /// <summary>Наступний стан рахунку.</summary>
    /// <param name="current">Поточний стан.</param>
    /// <param name="total">Підсумок рахунку.</param>
    /// <returns>Новий стан.</returns>
    public static BillingStatus NextStatusOf(
        BillingStatus current,
        decimal total)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(total);
        return current switch
        {
            BillingStatus.Issued when total > 0m => BillingStatus.Paid,
            BillingStatus.Paid => BillingStatus.Closed,
            _ => current,
        };
    }
}
