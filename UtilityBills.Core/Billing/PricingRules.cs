namespace UtilityBills.Core.Billing;

/// <summary>Іменовані правила знижок і плати за доставку.</summary>
public static class PricingRules
{
    /// <summary>Мінімальна кількість позицій для гуртової знижки.</summary>
    public const int BulkQuantity = 10;

    /// <summary>Частка гуртової знижки.</summary>
    public const decimal BulkRate = 0.05m;

    /// <summary>Частка знижки постійного клієнта.</summary>
    public const decimal RegularClientRate = 0.07m;

    /// <summary>Поріг суми для великого замовлення.</summary>
    public const decimal LargeOrderFrom = 5000m;

    /// <summary>Частка знижки для великого замовлення.</summary>
    public const decimal LargeOrderRate = 0.10m;

    /// <summary>Поріг суми для середнього замовлення.</summary>
    public const decimal MediumOrderFrom = 2000m;

    /// <summary>Частка знижки для середнього замовлення.</summary>
    public const decimal MediumOrderRate = 0.05m;

    /// <summary>Частка знижки у вихідний день (неділю).</summary>
    public const decimal WeekendRate = 0.02m;

    /// <summary>Частка знижки за купоном розпродажу.</summary>
    public const decimal CouponSaleRate = 0.10m;

    /// <summary>Фіксована сума знижки за купоном.</summary>
    public const decimal CouponFixed = 200m;

    /// <summary>Мінімальна сума замовлення для застосування фіксованого купона.</summary>
    public const decimal CouponFixedFrom = 1000m;

    /// <summary>Поріг суми для безкоштовної доставки.</summary>
    public const decimal FreeDeliveryFrom = 1000m;

    /// <summary>Код купона на знижку розпродажу.</summary>
    public const string SaleCoupon = "SALE10";

    /// <summary>Код купона на фіксовану знижку.</summary>
    public const string FixedCoupon = "MINUS200";
}
