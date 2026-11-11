using UtilityBills.Core.Domain;
using Xunit;

namespace UtilityBills.Tests;

public class BillTransitionTests
{
    [Fact]
    public void Issue_DraftWithLine_BecomesIssued()
    {
        var bill = NewDraft();

        bill.Issue();

        Assert.Equal(BillStatus.Issued, bill.Status);
    }

    [Fact]
    public void Pay_Issued_BecomesPaid()
    {
        var bill = NewIssued();

        bill.Pay();

        Assert.Equal(BillStatus.Paid, bill.Status);
    }

    [Fact]
    public void Pay_Overdue_BecomesPaid()
    {
        var bill = NewIssued();
        bill.MarkOverdue(new DateOnly(2026, 12, 1));

        bill.Pay();

        Assert.Equal(BillStatus.Paid, bill.Status);
    }

    [Fact]
    public void Cancel_Issued_BecomesCancelled()
    {
        var bill = NewIssued();

        bill.Cancel();

        Assert.Equal(BillStatus.Cancelled, bill.Status);
    }

    [Fact]
    public void Pay_Draft_ThrowsInvalidOperation()
    {
        var bill = NewDraft();

        Assert.Throws<InvalidOperationException>(() => bill.Pay());
    }

    [Fact]
    public void Issue_AlreadyIssued_ThrowsInvalidOperation()
    {
        var bill = NewIssued();

        Assert.Throws<InvalidOperationException>(() => bill.Issue());
    }

    [Fact]
    public void Cancel_Paid_ThrowsInvalidOperation()
    {
        var bill = NewIssued();
        bill.Pay();

        Assert.Throws<InvalidOperationException>(() => bill.Cancel());
    }

    [Fact]
    public void MarkOverdue_Draft_ThrowsInvalidOperation()
    {
        var bill = NewDraft();

        Assert.Throws<InvalidOperationException>(
            () => bill.MarkOverdue(new DateOnly(2026, 12, 1)));
    }

    private static UtilityBill NewDraft()
    {
        var bill = new UtilityBill(1, 1, "2026-10", new DateOnly(2026, 11, 20));
        bill.AddLine(new UtilityBillLine(1, 10m, 2.64m));
        return bill;
    }

    private static UtilityBill NewIssued()
    {
        var bill = NewDraft();
        bill.Issue();
        return bill;
    }
}
