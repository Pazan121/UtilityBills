using UtilityBills.Core.Domain;
using UtilityBills.Core.Reporting;
using Xunit;

namespace UtilityBills.Tests;

public class HouseChargeReportTests
{
    private const string _house = "Київ, вул. Вишнева, 1";

    [Fact]
    public void Build_NoBillsInHouse_SaysThereAreNone()
    {
        var text = HouseChargeReport.Build(
            _house, new FakeReferenceData().GetSubscribers(), Array.Empty<UtilityBill>(), _ => 0m);

        Assert.Contains("Рахунків немає.", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Build_OneSubscriber_ShowsHisTotal()
    {
        var bills = new[] { NewBill(1, 1) };

        var text = HouseChargeReport.Build(
            _house, new FakeReferenceData().GetSubscribers(), bills, _ => 363m);

        Assert.Contains("Абонент 001: 363.00", text, StringComparison.Ordinal);
        Assert.Contains("Разом: 363.00 (1 рах.)", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Build_TwoSubscribers_OrdersByNameAndSums()
    {
        var bills = new[] { NewBill(1, 2), NewBill(2, 1), NewBill(3, 7) };

        var text = HouseChargeReport.Build(
            _house, new FakeReferenceData().GetSubscribers(), bills, _ => 100m);

        Assert.True(
            text.IndexOf("Абонент 001", StringComparison.Ordinal)
            < text.IndexOf("Абонент 002", StringComparison.Ordinal));
        Assert.Contains("Разом: 200.00 (2 рах.)", text, StringComparison.Ordinal);
        Assert.DoesNotContain("Абонент 007", text, StringComparison.Ordinal);
    }

    private static UtilityBill NewBill(int id, int subscriberId)
    {
        return new UtilityBill(id, subscriberId, "2026-10", new DateOnly(2026, 11, 20));
    }
}
