using UtilityBills.Core.Billing;
using UtilityBills.Core.Reporting;
using Xunit;

namespace UtilityBills.Tests;

public class BillReportTests
{
    [Fact]
    public void TextOf_ContainsTotalLine()
    {
        BillTotalRequest request = BuildSampleRequest();
        BillTotalResult result = BillTotalCalculator.Calculate(request);

        string text = BillReport.TextOf(request, result);

        Assert.Contains("Разом: 1292.70", text);
    }

    [Fact]
    public void Print_WritesReportToOutput()
    {
        StringWriter writer = new();
        BillReportPrinter printer = new(writer);

        printer.Print("Разом: 10.00");

        Assert.Contains("10.00", writer.ToString());
    }

    private static BillTotalRequest BuildSampleRequest()
    {
        return new BillTotalRequest(
            BillId: 1,
            Subscriber: new SubscriberInfo("Іваненко", "i@ex.com", true),
            Lines: new List<ChargeLine>
            {
                new() { ServiceCode = "WATER", Quantity = 12, Tariff = 100m },
                new() { ServiceCode = "GAS", Quantity = 1, Tariff = 250m },
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: null,
            Status: BillingStatus.Issued,
            DeliveryPrice: 60m);
    }
}
