// Тимчасовий Program.cs із завдання 4 (до INotifier).
using System.Globalization;
using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Pricing;
using UtilityBills.Core.Services;
using UtilityBills.Core.Storage;

InMemoryBillRepository repository = new();
IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
BillService service = new(repository, pricing);

UtilityBill bill = new(1, 1, "2026-10", new DateOnly(2026, 11, 20));
bill.AddLine(new UtilityBillLine(1, 20m, 25m));
service.Place(bill);
string total = service.TotalOf(bill).ToString("0.00", CultureInfo.CurrentCulture);
Console.WriteLine($"Сума: {total}");
