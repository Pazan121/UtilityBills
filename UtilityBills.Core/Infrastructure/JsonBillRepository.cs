using UtilityBills.Core.Abstractions;
using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Infrastructure;

/// <summary>Сховище рахунків із збереженням у файл JSON через окремі типи передачі даних.</summary>
public sealed class JsonBillRepository : IBillRepository
{
    private readonly string _path;
    private readonly Dictionary<int, UtilityBill> _bills = new();

    /// <summary>Initializes a new instance of the <see cref="JsonBillRepository"/> class.</summary>
    /// <param name="path">Шлях до файла JSON.</param>
    public JsonBillRepository(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _path = path;
        foreach (BillDto dto in JsonFile.ReadList<BillDto>(path))
        {
            UtilityBill bill = FromDto(dto);
            _bills[bill.Id] = bill;
        }
    }

    /// <inheritdoc/>
    public void Add(UtilityBill bill)
    {
        ArgumentNullException.ThrowIfNull(bill);
        if (!_bills.TryAdd(bill.Id, bill))
        {
            throw new InvalidOperationException($"Рахунок {bill.Id} уже існує.");
        }
    }

    /// <inheritdoc/>
    public UtilityBill? GetById(int id)
    {
        return _bills.TryGetValue(id, out UtilityBill? bill) ? bill : null;
    }

    /// <inheritdoc/>
    public IReadOnlyList<UtilityBill> GetAll()
    {
        return _bills.Values.ToList();
    }

    /// <summary>Записує всі рахунки у файл.</summary>
    public void Save()
    {
        JsonFile.WriteList(_path, _bills.Values.Select(ToDto).ToList());
    }

    private static BillDto ToDto(UtilityBill bill)
    {
        List<LineDto> lines = bill.Lines
            .Select(line => new LineDto(line.UtilityServiceId, line.Quantity, line.Tariff))
            .ToList();
        return new BillDto(
            bill.Id,
            bill.SubscriberId,
            bill.BillingPeriod,
            bill.DueDate,
            bill.Status,
            lines);
    }

    private static UtilityBill FromDto(BillDto dto)
    {
        UtilityBill bill = new(dto.Id, dto.SubscriberId, dto.BillingPeriod, dto.DueDate);
        foreach (LineDto line in dto.Lines)
        {
            bill.AddLine(new UtilityBillLine(line.UtilityServiceId, line.Quantity, line.Tariff));
        }

        RestoreStatus(bill, dto);
        return bill;
    }

    private static void RestoreStatus(UtilityBill bill, BillDto dto)
    {
        switch (dto.Status)
        {
            case BillStatus.Draft:
                break;
            case BillStatus.Issued:
                bill.Issue();
                break;
            case BillStatus.Paid:
                bill.Issue();
                bill.Pay();
                break;
            case BillStatus.Overdue:
                bill.Issue();
                bill.MarkOverdue(dto.DueDate.AddDays(1));
                break;
            case BillStatus.Cancelled:
                bill.Cancel();
                break;
            default:
                throw new InvalidOperationException($"Невідомий стан {dto.Status}.");
        }
    }
}
