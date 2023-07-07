using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// And <see cref="ILogger"/> implementation to log to a <see cref="System.Windows.Controls.TextBlock"/>.
/// </summary>
internal sealed class TextBlockLogger : ILogger
{
    [ThreadStatic]
    private static StringWriter? stringWriter;

    private readonly string name;
    private readonly TextBlockLoggerProcessor queueProcessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBlockLogger"/> class.
    /// </summary>
    /// <param name="name">The name of this <see cref="ILogger"/>.</param>
    /// <param name="loggerProcessor">The <see cref="TextBlockLoggerProcessor"/>.</param>
    /// <param name="formatter">The formatter to format log messages.</param>
    /// <param name="scopeProvider">A <see cref="IExternalScopeProvider"/>.</param>
    /// <param name="options">Options for this logger.</param>
    internal TextBlockLogger(
        string name,
        TextBlockLoggerProcessor loggerProcessor,
        TextBlockFormatter formatter,
        IExternalScopeProvider? scopeProvider,
        TextBlockLoggerOptions options)
    {
        this.name = name ?? throw new ArgumentNullException(nameof(name));
        queueProcessor = loggerProcessor;
        Formatter = formatter;
        ScopeProvider = scopeProvider;
        Options = options;
    }

    /// <summary>
    /// Gets or sets the <see cref="TextBlockFormatter"/> to use.
    /// </summary>
    internal TextBlockFormatter Formatter
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the <see cref="TextBlockLoggerOptions"/>.
    /// </summary>
    internal TextBlockLoggerOptions Options
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the <see cref="IExternalScopeProvider"/>.
    /// </summary>
    internal IExternalScopeProvider? ScopeProvider
    {
        get;
        set;
    }

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => ScopeProvider?.Push(state) ?? NullScope.Instance;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
        => logLevel != LogLevel.None;

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)
            || ScopeProvider == null)
        {
            return;
        }

        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        stringWriter ??= new StringWriter();
        var logEntry = new LogEntry<TState>(logLevel, name, eventId, state, exception, formatter);
        Formatter.Write(in logEntry, ScopeProvider, stringWriter);

        var sb = stringWriter.GetStringBuilder();
        if (sb.Length == 0)
        {
            return;
        }

        var computedAnsiString = sb.ToString();
        _ = sb.Clear();
        if (sb.Capacity > 1024)
        {
            sb.Capacity = 1024;
        }

        queueProcessor.EnqueueMessage(new LogMessageEntry(computedAnsiString));
    }
}