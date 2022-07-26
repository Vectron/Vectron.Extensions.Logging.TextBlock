using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// Allows custom log messages formatting.
/// </summary>
public abstract class TextBlockFormatter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextBlockFormatter"/> class.
    /// </summary>
    /// <param name="name">The name of this formatter.</param>
    protected TextBlockFormatter(string name)
        => Name = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>
    /// Gets the name associated with the console log formatter.
    /// </summary>
    public string Name
    {
        get;
    }

    /// <summary>
    /// Writes the log message to the specified TextWriter.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="scopeProvider">The provider of scope data.</param>
    /// <param name="textWriter">The string writer embedding ansi code for colors.</param>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter);
}