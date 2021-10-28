namespace VectronsLibrary.TextBlockLogger;

/// <summary>
/// Options for the built-in textblock formatter.
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
    /// Gets or sets format string used to format timestamp in logging messages. Defaults to <c>null</c>.
    /// </summary>
    public string? TimestampFormat
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether UTC timezone should be used to for timestamps in logging messages. Defaults to <c>true</c>.
    /// </summary>
    public bool UseUtcTimestamp
    {
        get;
        set;
    }
}