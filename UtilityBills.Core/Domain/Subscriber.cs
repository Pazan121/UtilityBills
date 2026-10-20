namespace UtilityBills.Core.Domain;

/// <summary>Абонент: незмінний довідник учасників нарахування.</summary>
public sealed record Subscriber
{
    /// <summary>Initializes a new instance of the <see cref="Subscriber"/> class.</summary>
    /// <param name="id">Ідентифікатор, більший за нуль.</param>
    /// <param name="fullName">Повне ім'я, не порожнє.</param>
    /// <param name="address">Адреса, не порожня.</param>
    /// <param name="email">Пошта, що містить символ @.</param>
    /// <param name="isPrivileged">Чи має абонент пільгу.</param>
    public Subscriber(
        int id,
        string fullName,
        string address,
        string email,
        bool isPrivileged = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!email.Contains('@'))
        {
            throw new ArgumentException(
                "Пошта має містити символ @.",
                nameof(email));
        }

        Id = id;
        FullName = fullName;
        Address = address;
        Email = email;
        IsPrivileged = isPrivileged;
    }

    /// <summary>Gets the ідентифікатор абонента.</summary>
    public int Id { get; }

    /// <summary>Gets the повне ім'я абонента.</summary>
    public string FullName { get; }

    /// <summary>Gets the адресу абонента.</summary>
    public string Address { get; }

    /// <summary>Gets the пошту абонента.</summary>
    public string Email { get; }

    /// <summary>Gets a value indicating whether абонент має пільгу.</summary>
    public bool IsPrivileged { get; }
}
