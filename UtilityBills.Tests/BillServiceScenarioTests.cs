using Microsoft.Extensions.Logging.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;
using UtilityBills.Core.Pricing;
using UtilityBills.Core.Services;
using UtilityBills.Core.Storage;
using Xunit;

namespace UtilityBills.Tests;

public class BillServiceScenarioTests
{
    private static readonly Subscriber _owner =
        new(1, "Абонент 001", "Київ, вул. Вишнева, 1", "a1@example.com");

    [Fact]
    public void Pay_ExactAmount_MarksBillPaid()
    {
        var (service, bill) = IssuedBill();

        service.Pay(bill.Id, 26.40m);

        Assert.Equal(BillStatus.Paid, bill.Status);
    }

    [Fact]
    public void Pay_AmountBelowTotal_ThrowsDomainRule()
    {
        var (service, bill) = IssuedBill();

        var ex = Assert.Throws<DomainRuleException>(() => service.Pay(bill.Id, 10m));

        Assert.Equal("bill.amount", ex.Rule);
    }

    [Fact]
    public void Pay_DraftBill_ThrowsDomainRule()
    {
        var (service, bill) = DraftBill();

        var ex = Assert.Throws<DomainRuleException>(() => service.Pay(bill.Id, 26.40m));

        Assert.Equal("bill.payable", ex.Rule);
    }

    [Fact]
    public void Cancel_DraftBill_CancelsIt()
    {
        var (service, bill) = DraftBill();

        service.Cancel(bill.Id, "помилкові дані");

        Assert.Equal(BillStatus.Cancelled, bill.Status);
    }

    [Fact]
    public void Cancel_PaidBill_ThrowsDomainRule()
    {
        var (service, bill) = IssuedBill();
        service.Pay(bill.Id, 26.40m);

        var ex = Assert.Throws<DomainRuleException>(
            () => service.Cancel(bill.Id, "пізно"));

        Assert.Equal("bill.cancellable", ex.Rule);
    }

    private static (BillService Service, UtilityBill Bill) DraftBill()
    {
        var repository = new InMemoryBillRepository();
        var service = new BillService(
            repository,
            new CeilingPricingPolicy(),
            new SpyNotifier(),
            NullLogger<BillService>.Instance);
        var bill = new UtilityBill(1, _owner.Id, "2026-10", new DateOnly(2026, 11, 20));
        bill.AddLine(new UtilityBillLine(1, 10m, 2.64m));
        repository.Add(bill);
        return (service, bill);
    }

    private static (BillService Service, UtilityBill Bill) IssuedBill()
    {
        var (service, bill) = DraftBill();
        bill.Issue();
        return (service, bill);
    }
}
