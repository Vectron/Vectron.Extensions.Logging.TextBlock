using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace VectronsLibrary.TextBlockLogger;

/// <summary>
/// And <see cref="ILogger"/> implementation to log to a <see cref="System.Windows.Controls.TextBlock"/>.
/// </summary>
internal class TextBlockLogger : ILogger
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
    internal TextBlockLogger(string name, TextBlockLoggerProcessor loggerProcessor)
    {
        this.name = name ?? throw new ArgumentNullException(nameof(name));
        queueProcessor = loggerProcessor;
    }

    /// <summary>
    /// Gets or sets the <see cref="TextBlockFormatter"/> to use.
    /// </summary>
    internal TextBlockFormatter? Formatter
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the <see cref="TextBlockLoggerOptions"/>.
    /// </summary>
    internal TextBlockLoggerOptions? Options
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
    public IDisposable BeginScope<TState>(TState state)
        => ScopeProvider?.Push(state) ?? NullScope.Instance;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
        => logLevel != LogLevel.None;

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)
            || Formatter == null
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

        var buildString = sb.ToString();
        _ = sb.Clear();
        if (sb.Capacity > 1024)
        {
            sb.Capacity = 1024;
        }

        (var logLevelString, var logLevelColors) = Formatter.LogLevelData(in logEntry);

        queueProcessor.EnqueueMessage(new LogMessageEntry(buildString, logLevelString, logLevelColors));
    }
}