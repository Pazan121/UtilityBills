using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Abstractions;

/// <summary>Сховище рахунків: відповідає лише за зберігання.</summary>
public interface IBillRepository
{
    /// <summary>Додає новий рахунок.</summary>
    /// <param name="bill">Рахунок.</param>
    void Add(UtilityBill bill);

    /// <summary>Шукає рахунок за ідентифікатором.</summary>
    /// <param name="id">Ідентифікатор рахунку.</param>
    /// <returns>Рахунок або null, якщо його немає.</returns>
    UtilityBill? GetById(int id);

    /// <summary>Повертає всі рахунки.</summary>
    /// <returns>Копія списку рахунків.</returns>
    IReadOnlyList<UtilityBill> GetAll();
}
