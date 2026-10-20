namespace UtilityBills.Core.Domain;

/// <summary>Комунальна послуга: незмінний довідник об'єктів обліку.</summary>
public sealed record UtilityService
{
    /// <summary>Initializes a new instance of the <see cref="UtilityService"/> class.</summary>
    /// <param name="id">Ідентифікатор, більший за нуль.</param>
    /// <param name="name">Назва послуги, не порожня.</param>
    /// <param name="tariff">Тариф за одиницю, більший за нуль.</param>
    public UtilityService(int id, string name, decimal tariff)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tariff);
        Id = id;
        Name = name;
        Tariff = tariff;
    }

    /// <summary>Gets the ідентифікатор послуги.</summary>
    public int Id { get; }

    /// <summary>Gets the назву послуги.</summary>
    public string Name { get; }

    /// <summary>Gets the тариф за одиницю обсягу.</summary>
    public decimal Tariff { get; }
}
