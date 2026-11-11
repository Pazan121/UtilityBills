using Microsoft.Extensions.Logging.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;
using UtilityBills.Core.Pricing;
using UtilityBills.Core.Services;
using UtilityBills.Core.Storage;
using Xunit;

namespace UtilityBills.Tests;

public class ChargeServiceTests
{
    private static readonly DateOnly DueDate = new(2026, 11, 20);

    [Fact]
    public void Charge_TwoReadings_IssuesBillWithStepCharge()
    {
        var fixture = new Fixture();
        fixture.AddReadings(1, 1000m, 1130m);

        var bill = fixture.Charges.Charge(1, 1, DueDate);

        Assert.Equal(BillStatus.Issued, bill.Status);
        Assert.Equal(2, bill.Lines.Count);
        Assert.Equal(363m, fixture.Bills.TotalOf(bill));
    }

    [Fact]
    public void Charge_PrivilegedSubscriber_AppliesPrivilegeToFirstStep()
    {
        var fixture = new Fixture();
        fixture.AddReadings(7, 1000m, 1130m);

        var bill = fixture.Charges.Charge(7, 1, DueDate);

        Assert.Equal(231m, fixture.Bills.TotalOf(bill));
    }

    [Fact]
    public void Charge_OneReading_ThrowsDomainRule()
    {
        var fixture = new Fixture();
        fixture.Readings.Add(new MeterReading(1, 1, new DateOnly(2026, 10, 1), 1000m));

        var ex = Assert.Throws<DomainRuleException>(
            () => fixture.Charges.Charge(1, 1, DueDate));

        Assert.Equal("charge.readings", ex.Rule);
    }

    [Fact]
    public void Charge_NoConsumption_ThrowsEmptyBill()
    {
        var fixture = new Fixture();
        fixture.AddReadings(1, 1000m, 1000m);

        var ex = Assert.Throws<DomainRuleException>(
            () => fixture.Charges.Charge(1, 1, DueDate));

        Assert.Equal("bill.empty", ex.Rule);
    }

    [Fact]
    public void Charge_UnknownSubscriber_ThrowsDomainRule()
    {
        var fixture = new Fixture();

        var ex = Assert.Throws<DomainRuleException>(
            () => fixture.Charges.Charge(99, 1, DueDate));

        Assert.Equal("subscriber.exists", ex.Rule);
    }

    [Fact]
    public void Scenario_ChargeThenPay_EndsPaid()
    {
        var fixture = new Fixture();
        fixture.AddReadings(1, 1000m, 1130m);
        var bill = fixture.Charges.Charge(1, 1, DueDate);

        fixture.Bills.Pay(bill.Id, 363m);

        Assert.Equal(BillStatus.Paid, bill.Status);
    }

    [Fact]
    public void Scenario_ChargeThenCancel_EndsCancelled()
    {
        var fixture = new Fixture();
        fixture.AddReadings(1, 1000m, 1130m);
        var bill = fixture.Charges.Charge(1, 1, DueDate);

        fixture.Bills.Cancel(bill.Id, "помилкові показники");

        Assert.Equal(BillStatus.Cancelled, bill.Status);
    }

    [Fact]
    public void Charge_Issued_NotifiesSubscriberOnce()
    {
        var fixture = new Fixture();
        fixture.AddReadings(1, 1000m, 1130m);

        fixture.Charges.Charge(1, 1, DueDate);

        Assert.Single(fixture.Notifier.Messages);
    }

    private sealed class Fixture
    {
        public Fixture()
        {
            Repository = new InMemoryBillRepository();
            Notifier = new SpyNotifier();
            Bills = new BillService(
                Repository,
                new CeilingPricingPolicy(),
                Notifier,
                NullLogger<BillService>.Instance);
            Charges = new ChargeService(Readings, new FakeReferenceData(), Repository, Bills);
        }

        public FakeMeterReadings Readings { get; } = new();

        public InMemoryBillRepository Repository { get; }

        public SpyNotifier Notifier { get; }

        public BillService Bills { get; }

        public ChargeService Charges { get; }

        public void AddReadings(int subscriberId, decimal previous, decimal current)
        {
            Readings.Add(new MeterReading(subscriberId, 1, new DateOnly(2026, 9, 1), previous));
            Readings.Add(new MeterReading(subscriberId, 1, new DateOnly(2026, 10, 1), current));
        }
    }
}
