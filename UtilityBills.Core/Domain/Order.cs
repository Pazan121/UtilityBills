using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilityBills.Core.Domain;

/// <summary>
/// Представляє рахунок за комунальні послуги з можливістю додавання позицій та розрахунку суми.
/// </summary>
public class Order
{
    private readonly List<string[]> _lines = new List<string[]>();

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="Order"/>.
    /// </summary>
    /// <param name="a">Унікальний ідентифікатор рахунку.</param>
    /// <param name="b">Ім'я абонента (платника).</param>
    public Order(string a, string b)
    {
        Id = a;
        SubscriberName = b;
        CreatedAt = DateTime.Now;
    }

    /// <summary>
    /// Отримує унікальний ідентифікатор рахунку.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Отримує ім'я абонента.
    /// </summary>
    public string SubscriberName { get; private set; }

    /// <summary>
    /// Отримує поточний статус рахунку (0 - новий, 1 - оплачений, 2 - відправлений, 3 - скасований).
    /// </summary>
    public int Status { get; private set; }

    /// <summary>
    /// Отримує дату та час створення рахунку.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Здійснює пошук рахунку за його ідентифікатором у заданій колекції.
    /// </summary>
    /// <param name="bills">Список рахунків для пошуку.</param>
    /// <param name="billId">Шуканий ідентифікатор.</param>
    /// <returns>Знайдений об'єкт <see cref="Order"/> або null, якщо рахунок не знайдено.</returns>
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
    /// Додає до рахунку новий рядок із комунальною послугою.
    /// </summary>
    /// <param name="serviceCode">Код комунальної послуги.</param>
    /// <param name="quantity">Спожитий обсяг послуги.</param>
    /// <param name="tariff">Тариф за одиницю послуги, грн.</param>
    public void AddLine(string serviceCode, int quantity, decimal tariff)
    {
        string[] t = new string[3];
        t[0] = serviceCode;
        t[1] = quantity.ToString();
        t[2] = tariff.ToString();
        _lines.Add(t);
    }

    /// <summary>
    /// Обчислює загальну вартість рахунку з урахуванням індивідуальних знижок та ставки ПДВ.
    /// </summary>
    /// <param name="isRegularSubscriber">Вказує, чи є абонент постійним клієнтом.</param>
    /// <returns>Кінцева сума до сплати з округленням до двох знаків.</returns>
    public decimal CalculateTotal(bool isRegularSubscriber)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int k = int.Parse(_lines[i][1]);
            decimal c = decimal.Parse(_lines[i][2]);
            total = total + (k * c);
            lineCount = lineCount + 1;
        }

        // Порядок нарахування знижок залежно від статусу та обсягу
        if (isRegularSubscriber == true && total > 1000)
        {
            total = total * 0.9m;
        }
        else if (total > 5000)
        {
            total = total * 0.85m;
        }
        else if (lineCount > 10)
        {
            total = total - 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        // Додавання податку ПДВ (ставка 20%)
        total = total + (total * 0.2m);
        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан рахунку, якщо перехід дозволений бізнес-правилами предметної області.
    /// </summary>
    /// <param name="newStatus">Цільовий стан рахунку.</param>
    /// <returns>
    /// true, якщо перехід виконано успішно; false, якщо він заборонений поточним станом.
    /// </returns>
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
    /// Перевіряє валідність даних рахунку за базовими критеріями.
    /// </summary>
    /// <returns>true, якщо всі поля заповнені коректно; інакше - false.</returns>
    public bool IsValid()
    {
        return Id != null &&
               Id != string.Empty &&
               SubscriberName != null &&
               SubscriberName.Length > 2 &&
               _lines.Count > 0 &&
               _lines.Count < 100 &&
               Status >= 0 &&
               Status <= 3;
    }

    /// <summary>
    /// Формує детальний текстовий звіт по нарахуваннях у рахунку.
    /// </summary>
    /// <returns>Багаторядковий рядок зі списком послуг та загальною сумою.</returns>
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