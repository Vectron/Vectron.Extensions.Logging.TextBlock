using Vectron.Extensions.Logging.TextBlock.Internal;

namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// Options for a <see cref="TextBlockLogger"/>.
/// </summary>
public class TextBlockLoggerOptions
{
    /// <summary>
    /// The default max queue length.
    /// </summary>
    internal const int DefaultMaxQueueLengthValue = 2500;

    private int maxQueuedMessages = DefaultMaxQueueLengthValue;
    private TextBlockLoggerQueueFullMode queueFullMode = TextBlockLoggerQueueFullMode.Wait;

    /// <summary>
    /// Gets or sets the name of the log message formatter to use. Defaults to "simple" /&gt;.
    /// </summary>
    public string? FormatterName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the max number of messages to keep in the <see cref="System.Windows.Controls.TextBlock"/>.
    /// </summary>
    public int MaxMessages { get; set; } = 100;

    /// <summary>
    /// Gets or sets the maximum number of enqueued messages. Defaults to 2500.
    /// </summary>
    public int MaxQueueLength
    {
        get => maxQueuedMessages;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxQueueLength)} must be larger than zero.");
            }

            maxQueuedMessages = value;
        }
    }

    /// <summary>
    /// Gets or sets the desired text block logger behavior when the queue becomes full. Defaults to <c>Wait</c>.
    /// </summary>
    public TextBlockLoggerQueueFullMode QueueFullMode
    {
        get => queueFullMode;
        set
        {
            if (value is not TextBlockLoggerQueueFullMode.Wait and not TextBlockLoggerQueueFullMode.DropWrite)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{value} is not a supported queue mode value.");
            }

            queueFullMode = value;
        }
    }
}
