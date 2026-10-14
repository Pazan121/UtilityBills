namespace UtilityBills.Core.Domain;

 

/// <summary>Абонент, якому нараховуються комунальні платежі.</summary>

public sealed class Subscriber

{

    public int Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;

}