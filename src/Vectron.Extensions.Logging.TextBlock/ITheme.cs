using Microsoft.Extensions.Logging;

namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// The theme to use for coloring.
/// </summary>
public interface ITheme
{
    /// <summary>
    /// Gets the unique name of this theme.
    /// </summary>
    string Name
    {
        get;
    }

    /// <summary>
    /// Get the color to use for the logging category.
    /// </summary>
    /// <param name="category">The category to colorize.</param>
    /// <returns>The ANSI color code, or empty string.</returns>
    string GetCategoryColor(string category);

    /// <summary>
    /// Get the color to use for the logging event id.
    /// </summary>
    /// <param name="eventId">The event id to colorize.</param>
    /// <returns>The ANSI color code, or empty string.</returns>
    string GetEventIdColor(EventId eventId);

    /// <summary>
    /// Get the color to use for the <see cref="Exception.Message"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to colorize.</param>
    /// <returns>The ANSI color code, or empty string.</returns>
    string GetExceptionColor(Exception exception);

    /// <summary>
    /// Get the color to use for the whole log message.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> to colorize.</param>
    /// <returns>The ANSI color code, or empty string.</returns>
    /// <remarks>This is only called when we color the whole line.</remarks>
    string GetLineColor(LogLevel logLevel);

    /// <summary>
    /// Get the color to use for the <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> to colorize.</param>
    /// <returns>The ANSI color code, or empty string.</returns>
    string GetLogLevelColor(LogLevel logLevel);

    /// <summary>
    /// Get the color to use for the log message.
    /// </summary>
    /// <param name="message">The message to colorize.</param>
    /// <returns>The ANSI color code, or empty string.</returns>
    string GetMessageColor(string message);

    /// <summary>
    /// Get the color to use for the log scope.
    /// </summary>
    /// <param name="scope">The scope to colorize.</param>
    /// <returns>The ANSI color code, or empty string.</returns>
    string GetScopeColor(object? scope);

    /// <summary>
    /// Get the color to use for the <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> to colorize.</param>
    /// <returns>The ANSI color code, or empty string.</returns>
    string GetTimeColor(DateTimeOffset dateTimeOffset);
}
