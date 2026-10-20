namespace UtilityBills.Core.Domain;

/// <summary>Рахунок за комунальні послуги: змінний агрегат зі станами.</summary>
public sealed class UtilityBill
{
    private readonly List<UtilityBillLine> _lines = new();

    /// <summary>Initializes a new instance of the <see cref="UtilityBill"/> class.</summary>
    /// <param name="id">Ідентифікатор, більший за нуль.</param>
    /// <param name="subscriberId">Ідентифікатор абонента, більший за нуль.</param>
    /// <param name="billingPeriod">Розрахунковий період, не порожній.</param>
    /// <param name="dueDate">Крайня дата оплати.</param>
    public UtilityBill(
        int id,
        int subscriberId,
        string billingPeriod,
        DateOnly dueDate)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(subscriberId);
        ArgumentException.ThrowIfNullOrWhiteSpace(billingPeriod);
        Id = id;
        SubscriberId = subscriberId;
        BillingPeriod = billingPeriod;
        DueDate = dueDate;
        Status = BillStatus.Draft;
    }

    /// <summary>Gets the ідентифікатор рахунку.</summary>
    public int Id { get; }

    /// <summary>Gets the ідентифікатор абонента.</summary>
    public int SubscriberId { get; }

    /// <summary>Gets the розрахунковий період.</summary>
    public string BillingPeriod { get; }

    /// <summary>Gets the крайню дату оплати.</summary>
    public DateOnly DueDate { get; }

    /// <summary>Gets the поточний стан; змінюється лише методами переходів.</summary>
    public BillStatus Status { get; private set; }

    /// <summary>Gets the рядки рахунку лише для читання.</summary>
    public IReadOnlyList<UtilityBillLine> Lines => _lines;

    /// <summary>Додає рядок; дозволено лише для чернетки.</summary>
    /// <param name="line">Рядок рахунку.</param>
    public void AddLine(UtilityBillLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (Status != BillStatus.Draft)
        {
            throw new InvalidOperationException(
                "Рядки додають лише до чернетки рахунку.");
        }

        _lines.Add(line);
    }

    /// <summary>Виставляє рахунок: Draft у Issued, якщо є хоча б один рядок.</summary>
    public void Issue()
    {
        if (Status != BillStatus.Draft)
        {
            throw ForbiddenTransition(BillStatus.Issued);
        }

        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("Рахунок порожній.");
        }

        Status = BillStatus.Issued;
    }

    /// <summary>Фіксує оплату: Issued або Overdue у Paid.</summary>
    public void Pay()
    {
        if (Status != BillStatus.Issued && Status != BillStatus.Overdue)
        {
            throw ForbiddenTransition(BillStatus.Paid);
        }

        Status = BillStatus.Paid;
    }

    /// <summary>Позначає прострочення: Issued у Overdue після дати оплати.</summary>
    /// <param name="today">Поточна дата.</param>
    public void MarkOverdue(DateOnly today)
    {
        if (Status != BillStatus.Issued)
        {
            throw ForbiddenTransition(BillStatus.Overdue);
        }

        if (today <= DueDate)
        {
            throw new InvalidOperationException("Строк оплати ще не минув.");
        }

        Status = BillStatus.Overdue;
    }

    /// <summary>Скасовує рахунок: Draft або Issued у Cancelled.</summary>
    public void Cancel()
    {
        if (Status != BillStatus.Draft && Status != BillStatus.Issued)
        {
            throw ForbiddenTransition(BillStatus.Cancelled);
        }

        Status = BillStatus.Cancelled;
    }

    private InvalidOperationException ForbiddenTransition(BillStatus target)
    {
        return new InvalidOperationException(
            $"Перехід зі стану {Status} у стан {target} заборонено.");
    }
}
