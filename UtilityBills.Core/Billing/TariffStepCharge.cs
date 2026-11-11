namespace UtilityBills.Core.Billing;

/// <summary>Частина споживання, яка оплачується за однією сходинкою тарифу.</summary>
/// <param name="Quantity">Обсяг споживання на сходинці, більший за нуль.</param>
/// <param name="Tariff">Тариф сходинки, грн за одиницю.</param>
public sealed record TariffStepCharge(decimal Quantity, decimal Tariff)
{
    private const decimal _centsInHryvnia = 100m;

    /// <summary>Gets the вартість сходинки, округлену до 0,01 грн угору.</summary>
    public decimal Amount =>
        decimal.Ceiling(Quantity * Tariff * _centsInHryvnia) / _centsInHryvnia;
}
