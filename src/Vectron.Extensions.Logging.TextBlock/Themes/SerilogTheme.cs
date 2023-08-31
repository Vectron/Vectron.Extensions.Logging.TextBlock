using Microsoft.Extensions.Logging;

namespace Vectron.Extensions.Logging.TextBlock.Themes;

/// <summary>
/// A <see cref="ITheme"/> that uses the Microsoft.Extensions.Logging.Console colors.
/// </summary>
internal sealed class SerilogTheme : ITheme
{
    /// <inheritdoc/>
    public string Name => "Serilog";

    /// <inheritdoc/>
    public string GetCategoryColor(string category) => "\x1b[38;5;0007m";

    /// <inheritdoc/>
    public string GetEventIdColor(EventId eventId) => "\x1b[38;5;0007m";

    /// <inheritdoc/>
    public string GetExceptionColor(Exception exception) => string.Empty;

    /// <inheritdoc/>
    public string GetLineColor(LogLevel logLevel) => GetLogLevelColor(logLevel);

    /// <inheritdoc/>
    public string GetLogLevelColor(LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => "\x1b[38;5;0007m\x1b[40m",
            LogLevel.Debug => "\x1b[38;5;0007m\x1b[40m",
            LogLevel.Information => "\x1b[38;5;0015m\x1b[40m",
            LogLevel.Warning => "\x1b[38;5;0011m\x1b[40m",
            LogLevel.Error => "\x1b[38;5;0015m\x1b[48;5;0196m",
            LogLevel.Critical => "\x1b[38;5;0015m\x1b[48;5;0196m",
            LogLevel.None => string.Empty,
            _ => string.Empty,
        };

    /// <inheritdoc/>
    public string GetMessageColor(string message) => "\x1b[38;5;0015m";

    /// <inheritdoc/>
    public string GetScopeColor(object? scope) => string.Empty;

    /// <inheritdoc/>
    public string GetTimeColor(DateTimeOffset dateTimeOffset) => string.Empty;
}
