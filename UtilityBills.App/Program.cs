// Проміжна версія завдань 5–6: логер і верхній обробник, але ще без аудиту та експорту.
#pragma warning disable CA1031 // Верхній обробник — єдине місце, де дозволено catch (Exception).
using System.Globalization;
using Microsoft.Extensions.Logging;
using UtilityBills.App;
using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;
using UtilityBills.Core.Pricing;
using UtilityBills.Core.Services;
using UtilityBills.Core.Storage;

using ILoggerFactory factory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "HH:mm:ss ";
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});
ILogger log = factory.CreateLogger("UtilityBills.App");

try
{
    RunApplication(args, factory);
    return 0;
}
catch (DomainRuleException ex)
{
    log.LogWarning("Правило {Rule}: {Message}", ex.Rule, ex.Message);
    return 1;
}
catch (Exception ex)
{
    log.LogCritical(ex, "Непередбачений збій");
    return 2;
}

static void RunApplication(string[] args, ILoggerFactory factory)
{
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
    BillService service = new(
        repository,
        pricing,
        notifier,
        factory.CreateLogger<BillService>());

    Subscriber subscriber = new(1, "Іваненко Іван", "Київ, вул. Вишнева, 1", "ivan@example.com");
    UtilityBill bill = new(1, subscriber.Id, "2026-10", new DateOnly(2026, 11, 20));
    repository.Add(bill);

    ReadLines(service, bill.Id);

    decimal total = service.Place(bill, subscriber);
    service.Pay(bill.Id, total + 10m);
    Console.WriteLine($"Сума: {total.ToString("0.00", CultureInfo.CurrentCulture)}");
}

static void ReadLines(BillService service, int billId)
{
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

        service.AddLine(billId, parsed.Value!);
    }
}
