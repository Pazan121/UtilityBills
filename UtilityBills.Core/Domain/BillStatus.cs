namespace UtilityBills.Core.Domain;

/// <summary>Стан рахунку за комунальні послуги.</summary>
public enum BillStatus
{
    /// <summary>Чернетка: рядки ще можна додавати.</summary>
    Draft = 0,

    /// <summary>Рахунок виставлено абоненту.</summary>
    Issued = 1,

    /// <summary>Рахунок оплачено.</summary>
    Paid = 2,

    /// <summary>Строк оплати минув.</summary>
    Overdue = 3,

    /// <summary>Рахунок скасовано.</summary>
    Cancelled = 4,
}
