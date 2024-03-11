using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Vectron.Extensions.Logging.TextBlock.Formatters;

/// <summary>
/// Allows custom log messages formatting.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TextBlockFormatter"/> class.
/// </remarks>
/// <param name="name">The name of this formatter.</param>
public abstract class TextBlockFormatter(string name)
{
    /// <summary>
    /// Gets the name associated with the console log formatter.
    /// </summary>
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>
    /// Writes the log message to the specified TextWriter.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="scopeProvider">The provider of scope data.</param>
    /// <param name="textWriter">The string writer embedding ANSI code for colors.</param>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter);
}
