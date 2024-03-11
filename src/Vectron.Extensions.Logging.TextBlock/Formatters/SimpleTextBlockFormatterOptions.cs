namespace Vectron.Extensions.Logging.TextBlock.Formatters;

/// <summary>
/// Options for the built-in default text block log formatter.
/// </summary>
public class SimpleTextBlockFormatterOptions : TextBlockFormatterOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleTextBlockFormatterOptions"/> class.
    /// </summary>
    public SimpleTextBlockFormatterOptions()
    {
    }

    /// <summary>
    /// Gets or sets determines when to use color when logging messages.
    /// </summary>
    public LoggerColorBehavior ColorBehavior
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether when <see langword="true" />, the entire message gets logged in a single line.
    /// </summary>
    public bool SingleLine
    {
        get;
        set;
    }
}
