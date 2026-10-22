namespace UtilityBills.Core.Auditing;

/// <summary>Журнал аудиту у файлі: володіє потоком, тому реалізує IDisposable.</summary>
public sealed class FileAuditLog : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="FileAuditLog"/> class.</summary>
    /// <param name="path">Шлях до файлу; рядки дописуються в кінець.</param>
    public FileAuditLog(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _writer = new StreamWriter(path, append: true);
    }

    /// <summary>Дописує рядок аудиту: операція, документ, час.</summary>
    /// <param name="operation">Назва операції.</param>
    /// <param name="documentId">Ідентифікатор документа.</param>
    public void Write(string operation, int documentId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        _writer.WriteLine($"{DateTimeOffset.Now:O};{operation};{documentId}");
    }

    /// <summary>Скидає буфер і закриває файл.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Flush();
        _writer.Dispose();
        _disposed = true;
    }
}
