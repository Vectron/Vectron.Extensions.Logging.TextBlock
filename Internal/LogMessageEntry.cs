using System;

namespace VectronsLibrary.TextBlockLogger;

/// <summary>
/// A container for a parsed log message.
/// </summary>
internal readonly struct LogMessageEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogMessageEntry"/> struct.
    /// </summary>
    /// <param name="message">The formatted message.</param>
    /// <param name="levelString">The text representation of the <see cref="Microsoft.Extensions.Logging.LogLevel"/>.</param>
    /// <param name="levelColors">The colors to use when displaying this message.</param>
    public LogMessageEntry(string message, string levelString, LevelColors levelColors)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        LevelString = levelString ?? throw new ArgumentNullException(nameof(levelString));
        LevelColors = levelColors;
    }

    /// <summary>
    /// Gets the colors to use when displaying this message.
    /// </summary>
    public LevelColors LevelColors
    {
        get;
    }

    /// <summary>
    /// Gets the text representation of the <see cref="Microsoft.Extensions.Logging.LogLevel"/>.
    /// </summary>
    public string LevelString
    {
        get;
    }

    /// <summary>
    /// Gets the formatted message.
    /// </summary>
    public string Message
    {
        get;
    }
}