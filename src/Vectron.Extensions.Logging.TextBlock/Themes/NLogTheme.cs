using Microsoft.Extensions.Logging;

namespace Vectron.Extensions.Logging.TextBlock.Themes;

/// <summary>
/// A <see cref="ITheme"/> that uses the Microsoft.Extensions.Logging.Console colors.
/// </summary>
internal sealed class NLogTheme : ITheme
{
    /// <inheritdoc/>
    public string Name => "NLog";

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
            LogLevel.Trace => "\u001b[90m\u001b[40m",
            LogLevel.Debug => "\u001b[37m\u001b[40m",
            LogLevel.Information => "\u001b[97m\u001b[40m",
            LogLevel.Warning => "\u001b[95m\u001b[40m",
            LogLevel.Error => "\u001b[93m\u001b[40m",
            LogLevel.Critical => "\u001b[91m\u001b[40m",
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
