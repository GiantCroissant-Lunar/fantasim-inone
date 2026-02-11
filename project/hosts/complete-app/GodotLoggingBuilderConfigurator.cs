using Crosscut.Logging;
using Microsoft.Extensions.Logging;

namespace FantaSim.App;

public sealed class GodotLoggingBuilderConfigurator : ILoggingBuilderConfigurator
{
    public int Priority => 0;

    public void Configure(ILoggingBuilder builder)
    {
        builder.AddProvider(new GodotLoggerProvider());
        builder.SetMinimumLevel(LogLevel.Debug);
    }
}
