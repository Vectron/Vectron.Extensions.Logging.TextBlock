namespace VectronsLibrary.TextBlockLogger;

/// <summary>
/// Determines the console logger behavior when the queue becomes full.
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