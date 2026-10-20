namespace UtilityBills.Core.Abstractions;

/// <summary>Сповіщення абонента (пошта, SMS тощо).</summary>
public interface INotifier
{
    /// <summary>Надсилає повідомлення одержувачу.</summary>
    /// <param name="recipient">Адреса одержувача.</param>
    /// <param name="message">Текст повідомлення.</param>
    void Notify(string recipient, string message);
}
