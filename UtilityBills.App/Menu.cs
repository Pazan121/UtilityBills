using System.Globalization;
using Microsoft.Extensions.Logging;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;
using UtilityBills.Core.Reporting;

namespace UtilityBills.App;

internal sealed class Menu
{
    private const int _paymentTermDays = 20;
    private const string _dateFormat = "yyyy-MM-dd";

    private static readonly string[] _menuLines =
    {
        "1. Внести показник лічильника",
        "2. Нарахувати платіж",
        "3. Оплатити рахунок",
        "4. Скасувати рахунок",
        "5. Знайти рахунки",
        "6. Звіт «нарахування за будинком»",
        "0. Вихід",
    };

    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly MenuServices _services;
    private readonly ILogger<Menu> _log;

    public Menu(TextReader input, TextWriter output, MenuServices services, ILogger<Menu> log)
    {
        _input = input;
        _output = output;
        _services = services;
        _log = log;
    }

    public void Run()
    {
        try
        {
            while (true)
            {
                foreach (string line in _menuLines)
                {
                    _output.WriteLine(line);
                }

                string choice = ReadText("Пункт меню: ");
                if (choice == "0")
                {
                    return;
                }

                Execute(choice);
            }
        }
        catch (EndOfStreamException)
        {
            _log.LogInformation("Введення завершено");
        }
    }

    private static string Format(decimal value)
    {
        return value.ToString("0.00", CultureInfo.InvariantCulture);
    }

    private void Execute(string choice)
    {
        _log.LogInformation("Пункт меню {Choice}: початок", choice);
        try
        {
            Dispatch(choice);
            _services.Save();
            _log.LogInformation("Пункт меню {Choice}: завершено", choice);
        }
        catch (DomainRuleException ex)
        {
            _output.WriteLine($"Відхилено: {ex.Message}");
            _log.LogWarning("Пункт меню {Choice} відхилено: {Rule}", choice, ex.Rule);
            LogStorageFailure(ex);
        }
    }

    private void LogStorageFailure(DomainRuleException ex)
    {
        if (ex.InnerException is IOException)
        {
            _log.LogError(ex, "Помилка сховища під час {Rule}", ex.Rule);
        }
    }

    private void Dispatch(string choice)
    {
        switch (choice)
        {
            case "1":
                SubmitReading();
                break;
            case "2":
                Charge();
                break;
            case "3":
                Pay();
                break;
            case "4":
                Cancel();
                break;
            case "5":
                Find();
                break;
            case "6":
                Report();
                break;
            default:
                _output.WriteLine("Невідомий пункт меню.");
                break;
        }
    }

    private void SubmitReading()
    {
        int subscriberId = ReadInt("Абонент (№): ");
        int serviceId = ReadInt("Послуга (№): ");
        DateOnly date = ReadDate("Дата (рррр-мм-дд): ");
        decimal value = ReadDecimal("Показник: ");
        MeterReading reading = _services.Meters.Submit(subscriberId, serviceId, date, value);
        _output.WriteLine($"Збережено показник {Format(reading.Value)}");
    }

    private void Charge()
    {
        int subscriberId = ReadInt("Абонент (№): ");
        int serviceId = ReadInt("Послуга (№): ");
        DateOnly due = _services.Clock.Today.AddDays(_paymentTermDays);
        UtilityBill bill = _services.Charges.Charge(subscriberId, serviceId, due);
        decimal total = _services.Bills.TotalOf(bill);
        _output.WriteLine($"Рахунок {bill.Id}, період {bill.BillingPeriod}: {Format(total)} грн");
    }

    private void Pay()
    {
        int billId = ReadInt("Рахунок (№): ");
        decimal amount = ReadDecimal("Сума: ");
        _services.Bills.Pay(billId, amount);
        _output.WriteLine("Оплату прийнято.");
    }

    private void Cancel()
    {
        int billId = ReadInt("Рахунок (№): ");
        string reason = ReadText("Причина: ");
        _services.Bills.Cancel(billId, reason);
        _output.WriteLine("Рахунок скасовано.");
    }

    private void Find()
    {
        int? subscriberId = ReadOptionalInt("Абонент (№, Enter — будь-який): ");
        BillStatus? status = ReadOptionalStatus("Стан (Draft, Issued, Paid, Overdue, Cancelled): ");
        IReadOnlyList<UtilityBill> found = _services.Search.Find(subscriberId, status);
        foreach (UtilityBill bill in found)
        {
            string total = Format(_services.Bills.TotalOf(bill));
            string text = $"№{bill.Id}, абонент {bill.SubscriberId}, {bill.Status}, {total} грн";
            _output.WriteLine(text);
        }

        _output.WriteLine($"Знайдено: {found.Count}");
    }

    private void Report()
    {
        string address = ReadText("Адреса будинку: ");
        string text = HouseChargeReport.Build(
            address,
            _services.Reference.GetSubscribers(),
            _services.Search.Find(null, null),
            _services.Bills.TotalOf);
        _output.Write(text);
    }

    private string ReadText(string prompt)
    {
        _output.Write(prompt);
        return _input.ReadLine() ?? throw new EndOfStreamException();
    }

    private int ReadInt(string prompt)
    {
        CultureInfo culture = CultureInfo.InvariantCulture;
        while (true)
        {
            string text = ReadText(prompt);
            if (int.TryParse(text, NumberStyles.None, culture, out int value) && value > 0)
            {
                return value;
            }

            _output.WriteLine("Введіть ціле число більше 0.");
        }
    }

    private int? ReadOptionalInt(string prompt)
    {
        string text = ReadText(prompt);
        CultureInfo culture = CultureInfo.InvariantCulture;
        bool parsed = int.TryParse(text, NumberStyles.None, culture, out int value);
        return parsed ? value : null;
    }

    private BillStatus? ReadOptionalStatus(string prompt)
    {
        string text = ReadText(prompt);
        bool parsed = Enum.TryParse(text, true, out BillStatus status) && Enum.IsDefined(status);
        return parsed ? status : null;
    }

    private decimal ReadDecimal(string prompt)
    {
        CultureInfo culture = CultureInfo.InvariantCulture;
        while (true)
        {
            string text = ReadText(prompt);
            if (decimal.TryParse(text, NumberStyles.AllowDecimalPoint, culture, out decimal value)
                && value >= 0m)
            {
                return value;
            }

            _output.WriteLine("Введіть число з крапкою, не менше 0 (наприклад, 1130.5).");
        }
    }

    private DateOnly ReadDate(string prompt)
    {
        CultureInfo culture = CultureInfo.InvariantCulture;
        while (true)
        {
            string text = ReadText(prompt);
            DateOnly value;
            if (DateOnly.TryParseExact(text, _dateFormat, culture, DateTimeStyles.None, out value))
            {
                return value;
            }

            _output.WriteLine("Введіть дату у форматі рррр-мм-дд.");
        }
    }
}
