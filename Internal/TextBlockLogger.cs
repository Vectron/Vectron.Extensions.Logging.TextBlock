using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;

namespace VectronsLibrary.TextBlockLogger
{
    internal class TextBlockLogger : ILogger
    {
        [ThreadStatic]
        private static StringWriter? t_stringWriter;

        private readonly string name;
        private readonly TextBlockLoggerProcessor queueProcessor;

        internal TextBlockLogger(string name, TextBlockLoggerProcessor loggerProcessor)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            queueProcessor = loggerProcessor;
        }

        internal TextBlockFormatter? Formatter
        {
            get;
            set;
        }

        internal TextBlockLoggerOptions? Options
        {
            get;
            set;
        }

        internal IExternalScopeProvider? ScopeProvider
        {
            get;
            set;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

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
            t_stringWriter ??= new StringWriter();
            LogEntry<TState> logEntry = new LogEntry<TState>(logLevel, name, eventId, state, exception, formatter);
            Formatter.Write(in logEntry, ScopeProvider, t_stringWriter);

            var sb = t_stringWriter.GetStringBuilder();
            if (sb.Length == 0)
            {
                return;
            }
            string buildString = sb.ToString();
            _ = sb.Clear();
            if (sb.Capacity > 1024)
            {
                sb.Capacity = 1024;
            }

            (string logLevelString, LevelColors logLevelColors) = Formatter.LogLevelData(in logEntry);

            queueProcessor.EnqueueMessage(new LogMessageEntry(buildString, logLevelString, logLevelColors));
        }
    }
}