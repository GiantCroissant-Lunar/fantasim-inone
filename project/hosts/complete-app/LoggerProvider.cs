using Godot;
using Microsoft.Extensions.Logging;

namespace FantaSim.App.Godot;

public sealed class LoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new Logger(categoryName);
    public void Dispose() { }
}

public sealed class Logger : ILogger
{
    private readonly string _category;

    public Logger(string categoryName)
    {
        // Use short name: "FantaSim.App.DockManager" -> "DockManager"
        var lastDot = categoryName.LastIndexOf('.');
        _category = lastDot >= 0 ? categoryName[(lastDot + 1)..] : categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Debug;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);

        if (logLevel >= LogLevel.Error)
        {
            GD.PrintErr($"[{_category}] {message}");
            if (exception is not null)
                GD.PrintErr($"[{_category}] {exception}");
        }
        else if (logLevel >= LogLevel.Warning)
        {
            GD.Print($"[{_category}] WARN: {message}");
        }
        else
        {
            GD.Print($"[{_category}] {message}");
        }
    }
}
