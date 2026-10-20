using System.Globalization;
using UtilityBills.App;
using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Pricing;
using UtilityBills.Core.Services;
using UtilityBills.Core.Storage;

// Композиційний корінь: єдине місце, де створюються конкретні реалізації.
string customerType = args.Length > 0 ? args[0] : "regular";
Dictionary<string, IPricingPolicy> policies = new()
{
    ["regular"] = new DiscountPricingPolicy(0.05m),
    ["privileged"] = new PrivilegedPricingPolicy(),
    ["employee"] = new DiscountPricingPolicy(0.30m),
};
IPricingPolicy pricing = policies.TryGetValue(customerType, out IPricingPolicy? found)
    ? found
    : new StandardPricingPolicy();

InMemoryBillRepository repository = new();
INotifier notifier = new ConsoleNotifier();
BillService service = new(repository, pricing, notifier);

Subscriber subscriber = new(1, "Іваненко Іван", "Київ, вул. Вишнева, 1", "ivan@example.com");
UtilityBill bill = new(1, subscriber.Id, "2026-10", new DateOnly(2026, 11, 20));
bill.AddLine(new UtilityBillLine(1, 20m, 25m));
service.Place(bill, subscriber);
string total = service.TotalOf(bill).ToString("0.00", CultureInfo.CurrentCulture);
Console.WriteLine($"Сума: {total}");
