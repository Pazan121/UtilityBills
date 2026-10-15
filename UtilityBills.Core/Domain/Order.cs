using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UtilityBills.Core.Domain;

/// <summary>
/// Представляє рахунок за комунальні послуги.
/// </summary>
public class Order
{
    private readonly List<string[]> _lines = new List<string[]>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Order"/> class.
    /// Ініціалізує новий рахунок.
    /// </summary>
    /// <param name="id">Ідентифікатор рахунку.</param>
    /// <param name="subscriberName">Ім'я абонента.</param>
    public Order(string id, string subscriberName)
    {
        Id = id;
        SubscriberName = subscriberName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>
    /// Gets отримує ідентифікатор рахунку.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Gets отримує ім'я абонента.
    /// </summary>
    public string SubscriberName { get; private set; }

    /// <summary>
    /// Gets отримує статус рахунку.
    /// </summary>
    public int Status { get; private set; }

    /// <summary>
    /// Gets отримує дату створення рахунку.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Знаходить рахунок за ідентифікатором.
    /// </summary>
    /// <param name="bills">Список рахунків.</param>
    /// <param name="billId">Ідентифікатор для пошуку.</param>
    /// <returns>Знайдений рахунок або null.</returns>
    public static Order? FindById(List<Order> bills, string billId)
    {
        for (int i = 0; i < bills.Count; i++)
        {
            if (bills[i].Id == billId)
            {
                return bills[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Додає послугу до рахунку.
    /// </summary>
    /// <param name="serviceCode">Код послуги.</param>
    /// <param name="quantity">Кількість спожитої послуги.</param>
    /// <param name="tariff">Тариф за одиницю.</param>
    public void AddLine(string serviceCode, int quantity, decimal tariff)
    {
        string[] line = new string[3];
        line[0] = serviceCode;
        line[1] = quantity.ToString(CultureInfo.InvariantCulture);
        line[2] = tariff.ToString(CultureInfo.InvariantCulture);

        _lines.Add(line);
    }

    /// <summary>
    /// Розраховує загальну суму рахунку.
    /// </summary>
    /// <param name="isRegularSubscriber">Чи є абонент постійним.</param>
    /// <returns>Загальна сума з урахуванням знижок і ПДВ.</returns>
    public decimal CalculateTotal(bool isRegularSubscriber)
    {
        decimal total = 0;
        int lineCount = 0;

        for (int i = 0; i < _lines.Count; i++)
        {
            int quantity = int.Parse(
                _lines[i][1],
                CultureInfo.InvariantCulture);

            decimal tariff = decimal.Parse(
                _lines[i][2],
                CultureInfo.InvariantCulture);

            total += quantity * tariff;
            lineCount++;
        }

        // Для постійних абонентів передбачена знижка.
        if (isRegularSubscriber && total > 1000)
        {
            total *= 0.9m;
        }
        else if (total > 5000)
        {
            total *= 0.85m;
        }
        else if (lineCount > 10)
        {
            total -= 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        // До суми додається ПДВ у розмірі 20%.
        total += total * 0.2m;

        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює статус рахунку, якщо перехід дозволений.
    /// </summary>
    /// <param name="newStatus">Новий статус.</param>
    /// <returns>Результат зміни статусу.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє правильність даних рахунку.
    /// </summary>
    /// <returns>true, якщо дані коректні.</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Id)
            && !string.IsNullOrWhiteSpace(SubscriberName)
            && SubscriberName.Length > 2
            && _lines.Count > 0
            && _lines.Count < 100
            && Status >= 0
            && Status <= 3;
    }

    /// <summary>
    /// Формує детальний текстовий звіт по нарахуваннях у рахунку.
    /// </summary>
    /// <returns>Багаторядковий рядок зі списком послуг та загальною сумою.</returns>
    public string BuildReport()
    {
        var report = new System.Text.StringBuilder();
        var culture = System.Globalization.CultureInfo.CurrentCulture;

        foreach (var line in _lines)
        {
            int quantity = int.Parse(line[1], System.Globalization.CultureInfo.InvariantCulture);
            decimal tariff = decimal.Parse(line[2], System.Globalization.CultureInfo.InvariantCulture);
            decimal amount = quantity * tariff;

            // Передаємо culture першим аргументом, щоб задовольнити CA1305 і зберегти кому для чисел
            report.AppendLine(culture, $"Послуга: {line[0]}; кількість: {quantity}; тариф: {tariff}; сума: {amount}");
        }

        report.AppendLine(culture, $"Разом: {CalculateTotal(false)}");
        return report.ToString();
    }
}
