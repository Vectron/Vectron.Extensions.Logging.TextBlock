namespace VectronsLibrary.TextBlockLogger;

/// <summary>
/// Options for the built-in default text block formatter.
/// </summary>
public class SimpleTextBlockFormatterOptions : TextBlockFormatterOptions
{
    /// <summary>
    /// Gets or sets determines when to use color when logging messages.
    /// </summary>
    public LoggerColorBehavior ColorBehavior
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether messages should be printed on a single line.
    /// </summary>
    public bool SingleLine
    {
        get;
        set;
    }
}