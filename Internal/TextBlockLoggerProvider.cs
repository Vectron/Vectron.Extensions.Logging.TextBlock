using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// A <see cref="ILoggerProvider"/> for <see cref="TextBlockLogger"/>.
/// </summary>
[ProviderAlias("TextBlock")]
internal class TextBlockLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly ConcurrentDictionary<string, TextBlockLogger> loggers;
    private readonly TextBlockLoggerProcessor messageQueue;
    private readonly IOptionsMonitor<TextBlockLoggerOptions> options;
    private readonly IDisposable optionsReloadToken;
    private ConcurrentDictionary<string, TextBlockFormatter> formatters = new();
    private IExternalScopeProvider scopeProvider = NullExternalScopeProvider.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBlockLoggerProvider"/> class.
    /// </summary>
    /// <param name="options">The <see cref="TextBlockLoggerOptions"/> monitor.</param>
    /// <param name="textBlockProvider">The <see cref="ITextBlockProvider"/>.</param>
    public TextBlockLoggerProvider(IOptionsMonitor<TextBlockLoggerOptions> options, ITextBlockProvider textBlockProvider)
        : this(options, textBlockProvider, Array.Empty<TextBlockFormatter>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBlockLoggerProvider"/> class.
    /// </summary>
    /// <param name="options">The <see cref="TextBlockLoggerOptions"/> monitor.</param>
    /// <param name="textBlockProvider">The <see cref="ITextBlockProvider"/>.</param>
    /// <param name="formatters">An <see cref="IEnumerable{T}"/> for getting the <see cref="TextBlockFormatter"/>.</param>
    public TextBlockLoggerProvider(IOptionsMonitor<TextBlockLoggerOptions> options, ITextBlockProvider textBlockProvider, IEnumerable<TextBlockFormatter> formatters)
    {
        this.options = options;
        loggers = new ConcurrentDictionary<string, TextBlockLogger>();
        SetFormatters(formatters);
        messageQueue = new TextBlockLoggerProcessor(
            textBlockProvider,
            options.CurrentValue.QueueFullMode,
            options.CurrentValue.MaxQueueLength);

        ReloadLoggerOptions(options.CurrentValue);
        optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string name)
    {
        if (options.CurrentValue.FormatterName == null
            || !formatters.TryGetValue(options.CurrentValue.FormatterName, out var logFormatter))
        {
            logFormatter = formatters[TextBlockFormatterNames.Simple];
        }

        return loggers.TryGetValue(name, out var logger)
            ? logger
            : loggers.GetOrAdd(name, new TextBlockLogger(name, messageQueue, logFormatter, scopeProvider, options.CurrentValue));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        optionsReloadToken?.Dispose();
        messageQueue.Dispose();
    }

    /// <inheritdoc />
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this.scopeProvider = scopeProvider;

        foreach (var logger in loggers)
        {
            logger.Value.ScopeProvider = scopeProvider;
        }
    }

    private void ReloadLoggerOptions(TextBlockLoggerOptions options)
    {
        if (options.FormatterName == null
            || !formatters.TryGetValue(options.FormatterName, out var logFormatter))
        {
            logFormatter = formatters[TextBlockFormatterNames.Simple];
        }

        messageQueue.FullMode = options.QueueFullMode;
        messageQueue.MaxQueueLength = options.MaxQueueLength;

        foreach (var logger in loggers)
        {
            logger.Value.Options = options;
            logger.Value.Formatter = logFormatter;
        }
    }

    private void SetFormatters(IEnumerable<TextBlockFormatter>? formatters = null)
    {
        var cd = new ConcurrentDictionary<string, TextBlockFormatter>(StringComparer.OrdinalIgnoreCase);
        var added = false;
        if (formatters != null)
        {
            foreach (var formatter in formatters)
            {
                _ = cd.TryAdd(formatter.Name, formatter);
                added = true;
            }
        }

        if (!added)
        {
            _ = cd.TryAdd(TextBlockFormatterNames.Simple, new SimpleTextBlockFormatter(new FormatterOptionsMonitor<SimpleTextBlockFormatterOptions>(new SimpleTextBlockFormatterOptions())));
        }

        this.formatters = cd;
    }
}