using Microsoft.Extensions.Logging;

namespace Vectron.Extensions.Logging.TextBlock.Themes;

/// <summary>
/// A <see cref="ITheme"/> that uses the Microsoft.Extensions.Logging.Console colors.
/// </summary>
internal sealed class NLogDarkTheme : ITheme
{
    /// <inheritdoc/>
    public string Name => "NLog-Dark";

    /// <inheritdoc/>
    public string GetCategoryColor(string category) => "\x1b[1;37m";

    /// <inheritdoc/>
    public string GetEventIdColor(EventId eventId) => "\x1b[1;37m";

    /// <inheritdoc/>
    public string GetExceptionColor(Exception exception) => "\x1b[1;37m";

    /// <inheritdoc/>
    public string GetLineColor(LogLevel logLevel) => GetLogLevelColor(logLevel);

    /// <inheritdoc/>
    public string GetLogLevelColor(LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => "\x1b[90m",
            LogLevel.Debug => "\x1b[37m",
            LogLevel.Information => "\x1b[97m",
            LogLevel.Warning => "\x1b[95m",
            LogLevel.Error => "\x1b[93m",
            LogLevel.Critical => "\x1b[91m",
            LogLevel.None => string.Empty,
            _ => string.Empty,
        };

    /// <inheritdoc/>
    public string GetMessageColor(string message) => "\x1b[1;37m";

    /// <inheritdoc/>
    public string GetScopeColor(object? scope) => "\x1b[1;37m";

    /// <inheritdoc/>
    public string GetTimeColor(DateTimeOffset dateTimeOffset) => "\x1b[1;37m";
}
