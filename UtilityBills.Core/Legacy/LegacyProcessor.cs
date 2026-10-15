#pragma warning disable IDE0011, SA1503, CA1002, CA1021, CA1307
using UtilityBills.Core.Billing;

namespace UtilityBills.Core.Legacy;

/// <summary>Навмисно поганий вихідний метод; буде видалений у завданні 5.</summary>
public static class LegacyProcessor
{
    /// <inheritdoc/>
    public static decimal Process(
        int billId,
        string subscriberName,
        string subscriberEmail,
        bool isRegular,
        List<ChargeLine> lines,
        DateOnly createdAt,
        string couponCode,
        string status,
        bool printToConsole,
        decimal deliveryPrice,
        out string newStatus)
    {
        newStatus = status;
        decimal total = 0;
        if (billId <= 0)
        {
            Console.WriteLine("Помилка: номер рахунку");
            return -1;
        }

        if (subscriberName == null || subscriberName == string.Empty)
        {
            Console.WriteLine("Помилка: немає абонента");
            return -1;
        }

        if (subscriberEmail == null || !subscriberEmail.Contains('@'))
        {
            Console.WriteLine("Помилка: пошта абонента");
            return -1;
        }

        if (lines == null || lines.Count == 0)
        {
            Console.WriteLine("Помилка: немає позицій");
            return -1;
        }

        if (status != "Issued" && status != "Paid" &&
            status != "Closed" && status != "Cancelled")
        {
            Console.WriteLine("Помилка: невідомий стан");
            return -1;
        }

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Quantity <= 0)
            {
                Console.WriteLine("Помилка: кількість");
                return -1;
            }

            if (lines[i].Tariff < 0)
            {
                Console.WriteLine("Помилка: тариф");
                return -1;
            }

            decimal sum = lines[i].Quantity * lines[i].Tariff;
            if (lines[i].Quantity >= 10)
                sum = sum * 0.95m;
            total = total + sum;
        }

        if (isRegular)
            total = total * 0.93m;
        if (total > 5000)
            total = total * 0.9m;
        else if (total > 2000)
            total = total * 0.95m;
        if (couponCode == "SALE10")
            total = total - (total * 0.1m);
        else if (couponCode == "MINUS200" && total > 1000)
            total = total - 200;
        if (createdAt.DayOfWeek == DayOfWeek.Sunday)
            total = total * 0.98m;
        if (total < 1000)
            total = total + deliveryPrice;
        if (total < 0)
            total = 0;
        total = Math.Round(total, 2);
        if (status == "Issued" && total > 0)
        {
            newStatus = "Paid";
        }
        else if (status == "Paid")
        {
            newStatus = "Closed";
        }
        else if (status == "Cancelled")
        {
            newStatus = "Cancelled";
            total = 0;
        }

        if (printToConsole)
        {
            Console.WriteLine("Рахунок № " + billId);
            Console.WriteLine("Абонент: " + subscriberName);
            Console.WriteLine("Пошта: " + subscriberEmail);
            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine(lines[i].ServiceCode + " x " +
                    lines[i].Quantity);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
    }
}
