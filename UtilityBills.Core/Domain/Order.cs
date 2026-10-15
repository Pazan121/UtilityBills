using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilityBills.Core.Domain;

// клас
public class Order
{
    private readonly List<string[]> _lines = new List<string[]>();

    public Order(string a, string b)
    {
        Id = a; // ставимо id
        SubscriberName = b; // ставимо cl
        CreatedAt = DateTime.Now; // ставимо дату
    }

    public string Id { get; private set; }

    public string SubscriberName { get; private set; }

    public int Status { get; private set; } // 0-новий,1-оплач,2-відпр,3-скасов

    public DateTime CreatedAt { get; private set; }

    // public string prim; // примітка, поки не треба

    // пошук
    public static Order FindById(List<Order> bills, string billId)
    {
        for (int i = 0; i < bills.Count; i++)
        {
            if (bills[i].Id == billId)
            {
                return bills[i];
            }
        }

        return string.Empty;
    }

    // конструктор

    // метод додавання
    public void AddLine(string serviceCode, int quantity, decimal tariff)
    {
        string[] t = new string[3];
        t[0] = serviceCode;
        t[1] = quantity.ToString();
        t[2] = tariff.ToString();
        _lines.Add(t); // додаємо t у ln
    }

    // ProcessData
    public decimal CalculateTotal(bool isRegularSubscriber)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int k = int.Parse(_lines[i][1]);
            decimal c = decimal.Parse(_lines[i][2]);
            total = total + (k * c); // додаємо до суми
            lineCount = lineCount + 1; // збільшуємо kolvo на одиницю
        }

        // if (sum1 > 500) { sum1 = sum1 - 50; } // стара знижка

        // ДОВГИЙ РЯДОК 1: наступні 3 рядки склеїти в один
        if (isRegularSubscriber == true && total > 1000)
        {
            total = total * 0.9m;
        }
        else if (total > 5000)
        {
            total = total * 0.85m;
        }
        else

        if (lineCount > 10)
        {
            total = total - 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        total = total + (total * 0.2m);
        return Math.Round(total, 2); // повертаємо sum1
    }

    // міняємо статус
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

        return false; // не можна
    }

    // перевірка
    public bool IsValid()
    {
        // ДОВГИЙ РЯДОК 2: наступні 2 рядки склеїти в один
        return Id != null &&
           Id != string.Empty &&
           SubscriberName != null &&
           SubscriberName.Length > 2 &&
           _lines.Count > 0 &&
           _lines.Count < 100 &&
           Status >= 0 &&
           Status <= 3;
    }

    // звіт
    public string BuildReport()
    {
        var report = new System.Text.StringBuilder();
        var culture = System.Globalization.CultureInfo.InvariantCulture;

        foreach (var line in _lines)
        {
            int quantity = int.Parse(line[1], culture);
            decimal tariff = decimal.Parse(line[2], culture);
            decimal amount = quantity * tariff;

            report.AppendLine(
                $"Товар: {line[0]}; кількість: {quantity}; ціна: {tariff}; сума: {amount}");
        }

        report.AppendLine($"Разом: {CalculateTotal(false)}");
        return report.ToString();
    }
}
