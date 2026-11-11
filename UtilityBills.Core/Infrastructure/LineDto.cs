namespace UtilityBills.Core.Infrastructure;

internal sealed record LineDto(int UtilityServiceId, decimal Quantity, decimal Tariff);
