namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// Options for the built-in text block formatter.
/// </summary>
public class TextBlockFormatterOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether scope should be included.
    /// </summary>
    public bool IncludeScopes
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets format string used to format timestamp in logging messages. Defaults to <see langword="null"/>.
    /// </summary>
    public string? TimestampFormat
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether UTC time zone should be used to for timestamps in
    /// logging messages. Defaults to <see langword="true"/>.
    /// </summary>
    public bool UseUtcTimestamp
    {
        get;
        set;
    }
}
