namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// Options for the single line console log formatter.
/// </summary>
public class ThemedTextBlockFormatterOptions : TextBlockFormatterOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the whole line should be colored.
    /// </summary>
    public bool ColorWholeLine
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the colors theme to use.
    /// </summary>
    public string? Theme
    {
        get;
        set;
    }
}
