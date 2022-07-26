using System;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// A container for a parsed log message.
/// </summary>
internal readonly struct LogMessageEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogMessageEntry"/> struct.
    /// </summary>
    /// <param name="message">The formatted message.</param>
    public LogMessageEntry(string message)
        => Message = message ?? throw new ArgumentNullException(nameof(message));

    /// <summary>
    /// Gets the formatted message.
    /// </summary>
    public string Message
    {
        get;
    }
}