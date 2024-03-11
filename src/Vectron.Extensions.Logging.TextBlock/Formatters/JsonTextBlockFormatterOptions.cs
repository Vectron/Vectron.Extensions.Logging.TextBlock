using System.Text.Json;

namespace Vectron.Extensions.Logging.TextBlock.Formatters;

/// <summary>
/// Options for the built-in json text block log formatter.
/// </summary>
public class JsonTextBlockFormatterOptions : TextBlockFormatterOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonTextBlockFormatterOptions"/> class.
    /// </summary>
    public JsonTextBlockFormatterOptions()
    {
    }

    /// <summary>
    /// Gets or sets JsonWriterOptions.
    /// </summary>
    public JsonWriterOptions JsonWriterOptions
    {
        get; set;
    }
}
