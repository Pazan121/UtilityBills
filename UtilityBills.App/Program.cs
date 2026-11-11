#pragma warning disable CA1031 // Верхній обробник — єдине місце, де дозволено catch (Exception).
using Microsoft.Extensions.Logging;
using UtilityBills.App;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Infrastructure;
using UtilityBills.Core.Pricing;
using UtilityBills.Core.Services;

Directory.CreateDirectory("data");
using FileLoggerProvider logProvider = new(Path.Combine("data", "app.log"));
using LoggerFactory factory = new();
factory.AddProvider(logProvider);
ILogger log = factory.CreateLogger("UtilityBills.App");
try
{
    return RunMenu(factory);
}
catch (Exception ex)
{
    log.LogError(ex, "Непередбачений збій");
    Console.WriteLine("Внутрішня помилка. Подробиці — у data/app.log.");
    return 2;
}

static int RunMenu(ILoggerFactory factory)
{
    SeedReferenceData reference = new();
    SystemClock clock = new();
    JsonBillRepository bills = new(Path.Combine("data", "bills.json"));
    JsonMeterReadingRepository readings = new(Path.Combine("data", "readings.json"));
    SeedDemoReadings(readings);
    BillService billService = new(
        bills,
        new CeilingPricingPolicy(),
        new ConsoleNotifier(),
        factory.CreateLogger<BillService>());
    ILogger<MeterReadingService> meterLog = factory.CreateLogger<MeterReadingService>();
    MenuServices services = new()
    {
        Meters = new MeterReadingService(readings, reference, clock, meterLog),
        Charges = new ChargeService(readings, reference, bills, billService),
        Bills = billService,
        Search = new BillSearchService(bills),
        Reference = reference,
        Clock = clock,
        Save = () =>
        {
            bills.Save();
            readings.Save();
        },
    };
    new Menu(Console.In, Console.Out, services, factory.CreateLogger<Menu>()).Run();
    return 0;
}

static void SeedDemoReadings(JsonMeterReadingRepository readings)
{
    if (readings.GetFor(1, 1).Count > 0)
    {
        return;
    }

    foreach (int subscriberId in new[] { 1, 7 })
    {
        readings.Add(new MeterReading(subscriberId, 1, new DateOnly(2026, 9, 1), 1000m));
        readings.Add(new MeterReading(subscriberId, 1, new DateOnly(2026, 10, 1), 1130m));
    }

    readings.Save();
}
