using System.Text.Json;
using System.Text.Json.Serialization;
using UtilityBills.Core.Errors;

namespace UtilityBills.Core.Infrastructure;

/// <summary>Читання й запис JSON-файлів; помилки диска стають винятками домену.</summary>
public static class JsonFile
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>Читає список із файла; відсутній файл дає порожній список.</summary>
    /// <typeparam name="T">Тип елемента.</typeparam>
    /// <param name="path">Шлях до файла.</param>
    /// <returns>Прочитані елементи.</returns>
    public static List<T> ReadList<T>(string path)
    {
        if (!File.Exists(path))
        {
            return new List<T>();
        }

        try
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
        }
        catch (IOException ex)
        {
            throw new DomainRuleException("storage.read", $"Не вдалося прочитати {path}", ex);
        }
        catch (JsonException ex)
        {
            throw new DomainRuleException("storage.format", $"Файл {path} пошкоджено", ex);
        }
    }

    /// <summary>Записує список у файл.</summary>
    /// <typeparam name="T">Тип елемента.</typeparam>
    /// <param name="path">Шлях до файла.</param>
    /// <param name="items">Елементи для запису.</param>
    public static void WriteList<T>(string path, IReadOnlyList<T> items)
    {
        try
        {
            File.WriteAllText(path, JsonSerializer.Serialize(items, _options));
        }
        catch (IOException ex)
        {
            throw new DomainRuleException("storage.write", $"Не вдалося записати {path}", ex);
        }
    }
}
