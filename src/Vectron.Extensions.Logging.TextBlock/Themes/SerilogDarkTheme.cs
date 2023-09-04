using Microsoft.Extensions.Logging;

namespace Vectron.Extensions.Logging.TextBlock.Themes;

/// <summary>
/// A <see cref="ITheme"/> that uses the Microsoft.Extensions.Logging.Console colors.
/// </summary>
internal sealed class SerilogDarkTheme : ITheme
{
    /// <inheritdoc/>
    public string Name => "Serilog-Dark";

    /// <inheritdoc/>
    public string GetCategoryColor(string category) => "\x1b[38;5;007m";

    /// <inheritdoc/>
    public string GetEventIdColor(EventId eventId) => "\x1b[38;5;007m";

    /// <inheritdoc/>
    public string GetExceptionColor(Exception exception) => "\x1b[38;5;015m";

    /// <inheritdoc/>
    public string GetLineColor(LogLevel logLevel) => GetLogLevelColor(logLevel);

    /// <inheritdoc/>
    public string GetLogLevelColor(LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => "\x1b[38;5;007m",
            LogLevel.Debug => "\x1b[38;5;007m",
            LogLevel.Information => "\x1b[38;5;015m",
            LogLevel.Warning => "\x1b[38;5;011m",
            LogLevel.Error => "\x1b[38;5;015m\x1b[48;5;196m",
            LogLevel.Critical => "\x1b[38;5;015m\x1b[48;5;196m",
            LogLevel.None => string.Empty,
            _ => string.Empty,
        };

    /// <inheritdoc/>
    public string GetMessageColor(string message) => "\x1b[38;5;015m";

    /// <inheritdoc/>
    public string GetScopeColor(object? scope) => "\x1b[38;5;015m";

    /// <inheritdoc/>
    public string GetTimeColor(DateTimeOffset dateTimeOffset) => "\x1b[38;5;015m";
}
