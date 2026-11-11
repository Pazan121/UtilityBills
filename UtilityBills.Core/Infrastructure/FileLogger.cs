using System.Globalization;
using Microsoft.Extensions.Logging;

namespace UtilityBills.Core.Infrastructure;

internal sealed class FileLogger : ILogger
{
    private const string TimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

    private static readonly AsyncLocal<string?> CurrentScenario = new();

    private readonly string _category;
    private readonly Action<string> _sink;

    public FileLogger(string category, Action<string> sink)
    {
        _category = category;
        _sink = sink;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        string? previous = CurrentScenario.Value;
        CurrentScenario.Value = state.ToString();
        return new ScenarioRestorer(previous);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string scenario = CurrentScenario.Value ?? _category;
        string stamp = DateTime.Now.ToString(TimeFormat, CultureInfo.InvariantCulture);
        string line = $"{stamp} [{logLevel}] {scenario}: {formatter(state, exception)}";
        _sink(exception is null ? line : $"{line} | {exception.GetType().Name}");
    }

    private sealed class ScenarioRestorer : IDisposable
    {
        private readonly string? _previous;

        public ScenarioRestorer(string? previous)
        {
            _previous = previous;
        }

        public void Dispose()
        {
            CurrentScenario.Value = _previous;
        }
    }
}
