using Crosscut.Logging;
using Microsoft.Extensions.Logging;

namespace FantaSim.App.Godot;

public sealed class LoggingBuilderConfigurator : ILoggingBuilderConfigurator
{
    public int Priority => 0;

    public void Configure(ILoggingBuilder builder)
    {
        builder.AddProvider(new LoggerProvider());
        builder.SetMinimumLevel(LogLevel.Debug);
    }
}
