using System.Globalization;
using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Billing;
using UtilityBills.Core.Domain;
using UtilityBills.Core.Errors;

namespace UtilityBills.Core.Services;

/// <summary>Сценарій UC-02: нарахувати платіж за двома останніми показниками.</summary>
public sealed class ChargeService
{
    private const int MinimumReadings = 2;

    private readonly IMeterReadingRepository _readings;
    private readonly IReferenceData _reference;
    private readonly IBillRepository _bills;
    private readonly BillService _billService;

    /// <summary>Initializes a new instance of the <see cref="ChargeService"/> class.</summary>
    /// <param name="readings">Сховище показників.</param>
    /// <param name="reference">Довідники абонентів і послуг.</param>
    /// <param name="bills">Сховище рахунків.</param>
    /// <param name="billService">Сервіс виставлення рахунків.</param>
    public ChargeService(
        IMeterReadingRepository readings,
        IReferenceData reference,
        IBillRepository bills,
        BillService billService)
    {
        ArgumentNullException.ThrowIfNull(readings);
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(bills);
        ArgumentNullException.ThrowIfNull(billService);
        _readings = readings;
        _reference = reference;
        _bills = bills;
        _billService = billService;
    }

    /// <summary>Створює й виставляє рахунок із рядками за сходинками тарифу.</summary>
    /// <param name="subscriberId">Ідентифікатор абонента.</param>
    /// <param name="utilityServiceId">Ідентифікатор послуги.</param>
    /// <param name="dueDate">Крайня дата оплати.</param>
    /// <returns>Виставлений рахунок.</returns>
    public UtilityBill Charge(int subscriberId, int utilityServiceId, DateOnly dueDate)
    {
        Subscriber subscriber = _reference.FindSubscriber(subscriberId)
            ?? throw new DomainRuleException(
                "subscriber.exists",
                $"Абонента {subscriberId} не знайдено");
        EnsureServiceExists(utilityServiceId);
        IReadOnlyList<MeterReading> history = _readings.GetFor(subscriberId, utilityServiceId);
        if (history.Count < MinimumReadings)
        {
            throw new DomainRuleException(
                "charge.readings",
                "Для нарахування потрібні щонайменше два показники");
        }

        MeterReading previous = history[^2];
        MeterReading current = history[^1];
        UtilityBill bill = new(NextBillId(), subscriberId, PeriodOf(current), dueDate);
        decimal consumption = current.Value - previous.Value;
        bool isPrivileged = subscriber.IsPrivileged;
        foreach (TariffStepCharge step in TariffCalculator.Steps(consumption, isPrivileged))
        {
            bill.AddLine(new UtilityBillLine(utilityServiceId, step.Quantity, step.Tariff));
        }

        _billService.Place(bill, subscriber);
        return bill;
    }

    private static string PeriodOf(MeterReading reading)
    {
        return reading.ReadingDate.ToString("yyyy-MM", CultureInfo.InvariantCulture);
    }

    private void EnsureServiceExists(int utilityServiceId)
    {
        if (_reference.FindService(utilityServiceId) is null)
        {
            throw new DomainRuleException(
                "service.exists",
                $"Послугу {utilityServiceId} не знайдено");
        }
    }

    private int NextBillId()
    {
        return _bills.GetAll().Select(bill => bill.Id).DefaultIfEmpty(0).Max() + 1;
    }
}
