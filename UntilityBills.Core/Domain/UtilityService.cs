namespace UtilityBills.Core.Domain;

 

/// <summary>Комунальна послуга та її тариф.</summary>

public sealed class UtilityService

{

    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Tariff { get; init; }

}