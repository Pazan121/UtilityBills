using UtilityBills.Core.Billing;
using UtilityBills.Core.Reporting;

BillTotalRequest request = BuildDemoRequest();
BillTotalResult result = BillTotalCalculator.Calculate(request);
string report = BillReport.TextOf(request, result);
new BillReportPrinter(Console.Out).Print(report);

static BillTotalRequest BuildDemoRequest()
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
