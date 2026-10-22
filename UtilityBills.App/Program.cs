using System.Globalization;
using UtilityBills.App;
using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;
using UtilityBills.Core.Pricing;
using UtilityBills.Core.Services;
using UtilityBills.Core.Storage;

// Композиційний корінь (проміжна версія завдання 4: без логера).
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
repository.Add(bill);

while (true)
{
    Console.Write("Код послуги (Enter — завершити): ");
    string? serviceId = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(serviceId))
    {
        break;
    }

    Console.Write("Обсяг: ");
    string? quantity = Console.ReadLine();
    Console.Write("Тариф: ");
    string? tariff = Console.ReadLine();

    Result<UtilityBillLine> parsed = BillLineParser.Parse(serviceId, quantity, tariff);
    if (!parsed.IsSuccess)
    {
        Console.WriteLine($"Помилка вводу: {parsed.Error}");
        continue;
    }

    service.AddLine(bill.Id, parsed.Value!);
}

decimal total = service.Place(bill, subscriber);
Console.WriteLine($"Сума: {total.ToString("0.00", CultureInfo.CurrentCulture)}");
