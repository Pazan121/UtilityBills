using Microsoft.Extensions.Logging.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;
using UtilityBills.Core.Services;
using Xunit;

namespace UtilityBills.Tests;

public class MeterReadingServiceTests
{
    private static readonly DateOnly Today = new(2026, 11, 1);

    [Fact]
    public void Submit_ValidReading_StoresIt()
    {
        var readings = new FakeMeterReadings();
        var service = NewService(readings);

        var saved = service.Submit(1, 1, new DateOnly(2026, 10, 1), 1000m);

        Assert.Equal(1000m, saved.Value);
        Assert.Single(readings.GetFor(1, 1));
    }

    [Fact]
    public void Submit_UnknownSubscriber_ThrowsDomainRule()
    {
        var service = NewService(new FakeMeterReadings());

        var ex = Assert.Throws<DomainRuleException>(
            () => service.Submit(99, 1, new DateOnly(2026, 10, 1), 10m));

        Assert.Equal("subscriber.exists", ex.Rule);
    }

    [Fact]
    public void Submit_UnknownService_ThrowsDomainRule()
    {
        var service = NewService(new FakeMeterReadings());

        var ex = Assert.Throws<DomainRuleException>(
            () => service.Submit(1, 99, new DateOnly(2026, 10, 1), 10m));

        Assert.Equal("service.exists", ex.Rule);
    }

    [Fact]
    public void Submit_FutureDate_ThrowsDomainRule()
    {
        var service = NewService(new FakeMeterReadings());

        var ex = Assert.Throws<DomainRuleException>(
            () => service.Submit(1, 1, Today.AddDays(1), 10m));

        Assert.Equal("reading.future", ex.Rule);
    }

    [Fact]
    public void Submit_ValueBelowPrevious_ThrowsRollback()
    {
        var readings = new FakeMeterReadings();
        readings.Add(new MeterReading(1, 1, new DateOnly(2026, 9, 1), 1000m));
        var service = NewService(readings);

        var ex = Assert.Throws<DomainRuleException>(
            () => service.Submit(1, 1, new DateOnly(2026, 10, 1), 999m));

        Assert.Equal("meter.rollback", ex.Rule);
    }

    [Fact]
    public void Submit_DateNotAfterPrevious_ThrowsDomainRule()
    {
        var readings = new FakeMeterReadings();
        readings.Add(new MeterReading(1, 1, new DateOnly(2026, 9, 1), 1000m));
        var service = NewService(readings);

        var ex = Assert.Throws<DomainRuleException>(
            () => service.Submit(1, 1, new DateOnly(2026, 9, 1), 1100m));

        Assert.Equal("reading.date", ex.Rule);
    }

    [Fact]
    public void Submit_NegativeValue_ThrowsOutOfRange()
    {
        var service = NewService(new FakeMeterReadings());

        Assert.Throws<ArgumentOutOfRangeException>(
            () => service.Submit(1, 1, new DateOnly(2026, 10, 1), -1m));
    }

    private static MeterReadingService NewService(FakeMeterReadings readings)
    {
        return new MeterReadingService(
            readings,
            new FakeReferenceData(),
            new FixedClock(Today),
            NullLogger<MeterReadingService>.Instance);
    }
}
