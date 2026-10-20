using UtilityBills.Core.Abstractions;

namespace UtilityBills.App;

/// <summary>Сповіщення через консоль (інфраструктура застосунку).</summary>
public sealed class ConsoleNotifier : INotifier
{
    /// <inheritdoc/>
    public void Notify(string recipient, string message)
    {
        Console.WriteLine($"[Лист для {recipient}] {message}");
    }
}
