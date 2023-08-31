using Microsoft.Extensions.Logging;

namespace Vectron.Extensions.Logging.TextBlock.Themes;

/// <summary>
/// A <see cref="ITheme"/> that uses the Microsoft.Extensions.Logging.Console colors.
/// </summary>
internal sealed class MELTheme : ITheme
{
    /// <inheritdoc/>
    public string Name => "MEL";

    /// <inheritdoc/>
    public string GetCategoryColor(string category) => string.Empty;

    /// <inheritdoc/>
    public string GetEventIdColor(EventId eventId) => string.Empty;

    /// <inheritdoc/>
    public string GetExceptionColor(Exception exception) => string.Empty;

    /// <inheritdoc/>
    public string GetLineColor(LogLevel logLevel) => GetLogLevelColor(logLevel);

    /// <inheritdoc/>
    public string GetLogLevelColor(LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => "\x1b[37m\x1b[40m",
            LogLevel.Debug => "\x1b[37m\x1b[40m",
            LogLevel.Information => "\x1b[32m\x1b[40m",
            LogLevel.Warning => "\x1b[1m\x1b[33m\x1b[40m",
            LogLevel.Error => "\x1b[30m\x1b[41m",
            LogLevel.Critical => "\x1b[1m\x1b[37m\x1b[41m",
            LogLevel.None => string.Empty,
            _ => string.Empty,
        };

    /// <inheritdoc/>
    public string GetMessageColor(string message) => string.Empty;

    /// <inheritdoc/>
    public string GetScopeColor(object? scope) => string.Empty;

    /// <inheritdoc/>
    public string GetTimeColor(DateTimeOffset dateTimeOffset) => string.Empty;
}
