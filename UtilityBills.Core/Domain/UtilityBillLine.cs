namespace UtilityBills.Core.Domain;

/// <summary>Рядок рахунку з обсягом послуги та тарифом.</summary>
public sealed class UtilityBillLine
{
    public int Id { get; init; }

    public int UtilityServiceId { get; init; }

    public decimal Quantity { get; init; }

    public decimal Tariff { get; init; }

    public decimal Amount => Quantity * Tariff;
}
