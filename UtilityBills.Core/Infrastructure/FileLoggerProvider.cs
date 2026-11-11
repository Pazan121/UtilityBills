using Microsoft.Extensions.Logging;

namespace UtilityBills.Core.Infrastructure;

/// <summary>
/// Провайдер журналу: рядки «час, рівень, сценарій, повідомлення» пишуться у файл.
/// </summary>
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly StreamWriter _writer;
    private readonly object _gate = new();

    /// <summary>Initializes a new instance of the <see cref="FileLoggerProvider"/> class.</summary>
    /// <param name="path">Шлях до файла журналу; рядки дописуються в кінець.</param>
    public FileLoggerProvider(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _writer = new StreamWriter(path, append: true) { AutoFlush = true };
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, WriteLine);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _writer.Dispose();
    }

    private void WriteLine(string line)
    {
        lock (_gate)
        {
            _writer.WriteLine(line);
        }
    }
}
