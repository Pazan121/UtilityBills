namespace UtilityBills.Core.Abstractions;

/// <summary>Джерело поточної дати; у тестах підміняється фіксованим.</summary>
public interface IClock
{
    /// <summary>Gets the поточну дату.</summary>
    DateOnly Today { get; }
}
