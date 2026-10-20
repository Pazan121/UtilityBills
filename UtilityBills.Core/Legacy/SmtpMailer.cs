#pragma warning disable CS1591, IDE0011, SA1503, SA1402, SA1649, CA1002, CA1822, CA1051
namespace UtilityBills.Core.Legacy;

/// <summary>Спрощений поштовий клієнт зі старого проєкту (заглушка).</summary>
public class SmtpMailer
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine("SMTP -> " + to + ": " + subject + " " + body);
    }
}
