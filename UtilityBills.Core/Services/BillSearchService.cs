using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Services;

/// <summary>Сценарій UC-05: знайти рахунки за абонентом і станом.</summary>
public sealed class BillSearchService
{
    private readonly IBillRepository _bills;

    /// <summary>Initializes a new instance of the <see cref="BillSearchService"/> class.</summary>
    /// <param name="bills">Сховище рахунків.</param>
    public BillSearchService(IBillRepository bills)
    {
        ArgumentNullException.ThrowIfNull(bills);
        _bills = bills;
    }

    /// <summary>Шукає рахунки; критерій null означає «будь-який».</summary>
    /// <param name="subscriberId">Ідентифікатор абонента або null.</param>
    /// <param name="status">Стан рахунку або null.</param>
    /// <returns>Рахунки за зростанням номера.</returns>
    public IReadOnlyList<UtilityBill> Find(int? subscriberId, BillStatus? status)
    {
        IEnumerable<UtilityBill> query = _bills.GetAll();
        if (subscriberId is not null)
        {
            query = query.Where(bill => bill.SubscriberId == subscriberId);
        }

        if (status is not null)
        {
            query = query.Where(bill => bill.Status == status);
        }

        return query.OrderBy(bill => bill.Id).ToList();
    }
}
