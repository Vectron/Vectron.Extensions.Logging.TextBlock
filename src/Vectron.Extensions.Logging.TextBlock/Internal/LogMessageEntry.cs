namespace Vectron.Extensions.Logging.TextBlock.Internal;

/// <summary>
/// A container for a parsed log message.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LogMessageEntry"/> struct.
/// </remarks>
/// <param name="message">The formatted message.</param>
internal readonly struct LogMessageEntry(string message)
{
    /// <summary>
    /// Gets the formatted message.
    /// </summary>
    public string Message => message ?? throw new ArgumentNullException(nameof(message));
}
