using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace VectronsLibrary.TextBlockLogger
{
    [ProviderAlias("TextBlock")]
    internal class TextBlockLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly ConcurrentDictionary<string, TextBlockLogger> loggers;
        private readonly TextBlockLoggerProcessor messageQueue;
        private readonly IOptionsMonitor<TextBlockLoggerOptions> options;
        private readonly IDisposable optionsReloadToken;
        private ConcurrentDictionary<string, TextBlockFormatter> formatters = new ConcurrentDictionary<string, TextBlockFormatter>();
        private IExternalScopeProvider scopeProvider = NullExternalScopeProvider.Instance;

        public TextBlockLoggerProvider(IOptionsMonitor<TextBlockLoggerOptions> options, ITextblockProvider textblockProvider, IEnumerable<TextBlockFormatter> formatters)
        {
            this.options = options;
            SetFormatters(formatters);
            loggers = new ConcurrentDictionary<string, TextBlockLogger>();
            messageQueue = new TextBlockLoggerProcessor(textblockProvider);

            ReloadLoggerOptions(options.CurrentValue);
            optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string name)
        {
            if (options.CurrentValue.FormatterName == null
                || !formatters.TryGetValue(options.CurrentValue.FormatterName, out TextBlockFormatter? logFormatter))
            {
                logFormatter = formatters[SimpleTextBlockFormatter.DefaultName];
            }

            return loggers.TryGetValue(name, out TextBlockLogger? logger)
                ? logger
                : loggers.GetOrAdd(name, new TextBlockLogger(name, messageQueue)
                {
                    Options = options.CurrentValue,
                    ScopeProvider = scopeProvider,
                    Formatter = logFormatter,
                });
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

            foreach (KeyValuePair<string, TextBlockLogger> logger in loggers)
            {
                logger.Value.ScopeProvider = scopeProvider;
            }
        }

        private void ReloadLoggerOptions(TextBlockLoggerOptions options)
        {
            if (options.FormatterName == null
                || !formatters.TryGetValue(options.FormatterName, out TextBlockFormatter? logFormatter))
            {
                logFormatter = formatters[SimpleTextBlockFormatter.DefaultName];
            }

            foreach (KeyValuePair<string, TextBlockLogger> logger in loggers)
            {
                logger.Value.Options = options;
                logger.Value.Formatter = logFormatter;
            }

            messageQueue.MaxMessages = options.MaxMessages;
        }

        private void SetFormatters(IEnumerable<TextBlockFormatter>? formatters = null)
        {
            var cd = new ConcurrentDictionary<string, TextBlockFormatter>(StringComparer.OrdinalIgnoreCase);
            bool added = false;
            if (formatters != null)
            {
                foreach (TextBlockFormatter formatter in formatters)
                {
                    _ = cd.TryAdd(formatter.Name, formatter);
                    added = true;
                }
            }

            if (!added)
            {
                _ = cd.TryAdd(SimpleTextBlockFormatter.DefaultName, new SimpleTextBlockFormatter(new FormatterOptionsMonitor<SimpleTextBlockFormatterOptions>(new SimpleTextBlockFormatterOptions())));
            }

            this.formatters = cd;
        }
    }
}