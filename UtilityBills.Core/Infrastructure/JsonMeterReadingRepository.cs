using UtilityBills.Core.Domain;

namespace UtilityBills.Core.Infrastructure;

/// <summary>Сховище показників із збереженням у файл JSON.</summary>
public sealed class JsonMeterReadingRepository : InMemoryMeterReadingRepository
{
    private readonly string _path;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonMeterReadingRepository"/> class.
    /// </summary>
    /// <param name="path">Шлях до файла JSON.</param>
    public JsonMeterReadingRepository(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _path = path;
        foreach (MeterReading reading in JsonFile.ReadList<MeterReading>(path))
        {
            Add(reading);
        }
    }

    /// <summary>Записує всі показники у файл.</summary>
    public void Save()
    {
        JsonFile.WriteList(_path, Items);
    }
}
