namespace VectronsLibrary.TextBlockLogger;

/// <summary>
/// Options for a <see cref="TextBlockLogger"/>.
/// </summary>
public class TextBlockLoggerOptions
{
    /// <summary>
    /// Gets or sets the name of the log message formatter to use. Defaults to "simple" />.
    /// </summary>
    public string? FormatterName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the max number of messages to keep in the <see cref="System.Windows.Controls.TextBlock"/>.
    /// </summary>
    public int MaxMessages
    {
        get;
        set;
    }
}