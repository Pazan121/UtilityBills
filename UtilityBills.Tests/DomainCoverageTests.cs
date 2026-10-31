using Microsoft.Extensions.Logging.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;
using UtilityBills.Core.Pricing;
using UtilityBills.Core.Reporting;
using UtilityBills.Core.Services;
using UtilityBills.Core.Storage;
using Xunit;

namespace UtilityBills.Tests;

public class DomainCoverageTests
{
    [Fact]
    public void Subscriber_EmailWithoutAt_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(
            () => new Subscriber(1, "Іван", "Київ", "no-at"));
    }

    [Fact]
    public void Subscriber_ZeroId_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Subscriber(0, "Іван", "Київ", "a@b.c"));
    }

    [Fact]
    public void Subscriber_ValidData_KeepsValues()
    {
        var subscriber = new Subscriber(1, "Іван", "Київ", "a@b.c", true);

        Assert.True(subscriber.IsPrivileged);
        Assert.Equal("a@b.c", subscriber.Email);
    }

    [Fact]
    public void UtilityService_ZeroTariff_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new UtilityService(1, "Вода", 0m));
    }

    [Fact]
    public void UtilityService_ValidData_KeepsValues()
    {
        var service = new UtilityService(1, "Вода", 25m);

        Assert.Equal("Вода", service.Name);
        Assert.Equal(25m, service.Tariff);
    }

    [Fact]
    public void UtilityBill_IssueWithoutLines_ThrowsInvalidOperation()
    {
        var bill = NewBill(1);

        Assert.Throws<InvalidOperationException>(() => bill.Issue());
    }

    [Fact]
    public void UtilityBill_MarkOverdueAfterDueDate_BecomesOverdue()
    {
        var bill = NewBill(1);
        bill.AddLine(new UtilityBillLine(1, 1m, 10m));
        bill.Issue();

        bill.MarkOverdue(new DateOnly(2026, 11, 21));

        Assert.Equal(BillStatus.Overdue, bill.Status);
    }

    [Fact]
    public void UtilityBill_MarkOverdueBeforeDueDate_ThrowsInvalidOperation()
    {
        var bill = NewBill(1);
        bill.AddLine(new UtilityBillLine(1, 1m, 10m));
        bill.Issue();

        Assert.Throws<InvalidOperationException>(
            () => bill.MarkOverdue(new DateOnly(2026, 11, 20)));
    }

    [Fact]
    public void UtilityBill_CancelDraft_BecomesCancelled()
    {
        var bill = NewBill(1);

        bill.Cancel();

        Assert.Equal(BillStatus.Cancelled, bill.Status);
    }

    [Fact]
    public void UtilityBill_PayDraft_ThrowsInvalidOperation()
    {
        var bill = NewBill(1);

        Assert.Throws<InvalidOperationException>(() => bill.Pay());
    }

    [Fact]
    public void ArchivedBill_FromPaidBill_KeepsIdAndStatus()
    {
        var bill = NewBill(3);
        bill.AddLine(new UtilityBillLine(1, 1m, 10m));
        bill.Issue();
        bill.Pay();

        var archived = new ArchivedBill(bill, new DateOnly(2026, 12, 1));

        Assert.Equal(3, archived.Id);
        Assert.Equal(BillStatus.Paid, archived.Status);
        Assert.Single(archived.Lines);
    }

    [Fact]
    public void ArchivedBill_FromDraftBill_ThrowsInvalidOperation()
    {
        var bill = NewBill(3);

        Assert.Throws<InvalidOperationException>(
            () => new ArchivedBill(bill, new DateOnly(2026, 12, 1)));
    }

    [Fact]
    public void PrivilegedPricingPolicy_Line_AppliesTenPercent()
    {
        var policy = new PrivilegedPricingPolicy();

        decimal actual = policy.PriceOf(new UtilityBillLine(1, 2m, 50m));

        Assert.Equal(90m, actual);
    }

    [Fact]
    public void StandardPricingPolicy_Line_ReturnsAmount()
    {
        var policy = new StandardPricingPolicy();

        decimal actual = policy.PriceOf(new UtilityBillLine(1, 2m, 50m));

        Assert.Equal(100m, actual);
    }

    [Fact]
    public void DiscountPricingPolicy_RateAboveLimit_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new DiscountPricingPolicy(0.6m));
    }

    [Fact]
    public void InMemoryBillRepository_DuplicateId_ThrowsInvalidOperation()
    {
        var repository = new InMemoryBillRepository();
        repository.Add(NewBill(1));

        Assert.Throws<InvalidOperationException>(
            () => repository.Add(NewBill(1)));
    }

    [Fact]
    public void InMemoryBillRepository_UnknownId_ReturnsNull()
    {
        var repository = new InMemoryBillRepository();
        repository.Add(NewBill(1));

        Assert.Null(repository.GetById(2));
        Assert.Single(repository.GetAll());
    }

    [Fact]
    public void Pay_ExactAmount_MarksBillPaid()
    {
        var repository = new FakeBillRepository();
        var bill = NewBill(1);
        bill.AddLine(new UtilityBillLine(1, 1m, 10m));
        bill.Issue();
        repository.Add(bill);
        var service = NewService(repository, 25m);

        service.Pay(1, 25m);

        Assert.Equal(BillStatus.Paid, bill.Status);
    }

    [Fact]
    public void Pay_AmountBelowTotal_ThrowsDomainRule()
    {
        var repository = new FakeBillRepository();
        var bill = NewBill(1);
        bill.AddLine(new UtilityBillLine(1, 1m, 10m));
        bill.Issue();
        repository.Add(bill);
        var service = NewService(repository, 25m);

        var ex = Assert.Throws<DomainRuleException>(
            () => service.Pay(1, 10m));

        Assert.Equal("bill.amount", ex.Rule);
    }

    [Fact]
    public void Pay_DraftBill_ThrowsDomainRule()
    {
        var repository = new FakeBillRepository();
        repository.Add(NewBill(1));
        var service = NewService(repository, 25m);

        var ex = Assert.Throws<DomainRuleException>(
            () => service.Pay(1, 25m));

        Assert.Equal("bill.payable", ex.Rule);
    }

    [Fact]
    public void Cancel_DraftBill_CancelsIt()
    {
        var repository = new FakeBillRepository();
        var bill = NewBill(1);
        repository.Add(bill);
        var service = NewService(repository, 25m);

        service.Cancel(1, "помилка в даних");

        Assert.Equal(BillStatus.Cancelled, bill.Status);
    }

    [Fact]
    public void AddLine_ToIssuedBill_ThrowsDomainRule()
    {
        var repository = new FakeBillRepository();
        var bill = NewBill(1);
        bill.AddLine(new UtilityBillLine(1, 1m, 10m));
        bill.Issue();
        repository.Add(bill);
        var service = NewService(repository, 25m);

        var ex = Assert.Throws<DomainRuleException>(
            () => service.AddLine(1, new UtilityBillLine(2, 1m, 5m)));

        Assert.Equal("bill.editable", ex.Rule);
    }

    [Fact]
    public void CountValidQuantities_MixedRows_CountsPositiveIntegers()
    {
        int actual = BillService.CountValidQuantities(
            new[] { "5", "x", "-1", "3", "0" });

        Assert.Equal(2, actual);
    }

    [Fact]
    public void BuildCsv_OneBill_ContainsHeaderAndRow()
    {
        var bill = NewBill(1);

        string csv = BillCsvReport.BuildCsv(
            new[] { bill }, _ => 25m);

        Assert.Contains("id;total;status", csv);
        Assert.Contains("1;25.00;Draft", csv);
    }

    [Fact]
    public void Result_OkAndFail_ExposeValueAndError()
    {
        var ok = Result<int>.Ok(5);
        var fail = Result<int>.Fail("помилка");

        Assert.True(ok.IsSuccess);
        Assert.Equal(5, ok.Value);
        Assert.False(fail.IsSuccess);
        Assert.Equal("помилка", fail.Error);
    }

    [Fact]
    public void DomainRuleException_WithInner_KeepsRuleAndInner()
    {
        var inner = new InvalidOperationException("перша причина");

        var ex = new DomainRuleException("bill.test", "обгортка", inner);

        Assert.Equal("bill.test", ex.Rule);
        Assert.Same(inner, ex.InnerException);
    }

    private static UtilityBill NewBill(int id)
    {
        return new UtilityBill(id, 1, "2026-10", new DateOnly(2026, 11, 20));
    }

    private static BillService NewService(
        FakeBillRepository repository, decimal price)
    {
        return new BillService(
            repository,
            new StubPricingPolicy(price),
            new RecordingNotifier(),
            NullLogger<BillService>.Instance);
    }
}
