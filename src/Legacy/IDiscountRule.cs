namespace UtilityBills.Legacy;

public interface IDiscountRule
{
    decimal? RateFor(Consumer client);
}
