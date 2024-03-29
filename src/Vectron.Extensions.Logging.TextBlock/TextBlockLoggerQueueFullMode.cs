namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// Determines the text block logger behavior when the queue becomes full.
/// </summary>
public enum TextBlockLoggerQueueFullMode
{
    /// <summary>
    /// Blocks the logging threads once the queue limit is reached.
    /// </summary>
    Wait,

    /// <summary>
    /// Drops new log messages when the queue is full.
    /// </summary>
    DropWrite,
}
