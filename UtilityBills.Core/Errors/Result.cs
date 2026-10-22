using System.Diagnostics.CodeAnalysis;

namespace UtilityBills.Core.Errors;

/// <summary>Результат очікуваної операції: значення або пояснення помилки.</summary>
/// <typeparam name="T">Тип значення.</typeparam>
[SuppressMessage(
    "Design",
    "CA1000:Do not declare static members on generic types",
    Justification = "Фабричні методи Ok і Fail є частиною контракту Result.")]
[SuppressMessage(
    "Performance",
    "CA1815:Override equals and operator equals on value types",
    Justification = "Результат не порівнюють між собою.")]
public readonly struct Result<T>
{
    private Result(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>Gets a value indicating whether операція вдалася.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets the значення; має сенс лише для вдалої операції.</summary>
    public T? Value { get; }

    /// <summary>Gets the пояснення помилки; для вдалої операції порожнє.</summary>
    public string Error { get; }

    /// <summary>Створює вдалий результат.</summary>
    /// <param name="value">Значення.</param>
    /// <returns>Вдалий результат.</returns>
    public static Result<T> Ok(T value)
    {
        return new Result<T>(true, value, string.Empty);
    }

    /// <summary>Створює невдалий результат.</summary>
    /// <param name="error">Пояснення помилки.</param>
    /// <returns>Невдалий результат.</returns>
    public static Result<T> Fail(string error)
    {
        return new Result<T>(false, default, error);
    }
}
