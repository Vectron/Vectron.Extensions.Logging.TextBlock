using Microsoft.Extensions.Logging;

namespace Vectron.Extensions.Logging.TextBlock.Themes;

/// <summary>
/// A <see cref="ITheme"/> that doesn't colorize anything.
/// </summary>
internal sealed class NoColorTheme : ITheme
{
    /// <inheritdoc/>
    public string Name => "None";

    /// <inheritdoc/>
    public string GetCategoryColor(string category) => string.Empty;

    /// <inheritdoc/>
    public string GetEventIdColor(EventId eventId) => string.Empty;

    /// <inheritdoc/>
    public string GetExceptionColor(Exception exception) => string.Empty;

    /// <inheritdoc/>
    public string GetLineColor(LogLevel logLevel) => string.Empty;

    /// <inheritdoc/>
    public string GetLogLevelColor(LogLevel logLevel) => string.Empty;

    /// <inheritdoc/>
    public string GetMessageColor(string message) => string.Empty;

    /// <inheritdoc/>
    public string GetScopeColor(object? scope) => string.Empty;

    /// <inheritdoc/>
    public string GetTimeColor(DateTimeOffset dateTimeOffset) => string.Empty;
}
