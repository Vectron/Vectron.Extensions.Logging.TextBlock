using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using VectronsLibrary.TextBlockLogger.Internal;

namespace VectronsLibrary.TextBlockLogger
{
    public class TextBlockLogger : ILogger
    {
        private static readonly string loglevelPadding = ": ";
        private static readonly string messagePadding;
        private static readonly string newLineWithMessagePadding;

        [ThreadStatic]
        private static StringBuilder logBuilder;

        private readonly Brush DefaultTextBlockColor = Brushes.Black;
        private readonly TextBlockLoggerProcessor queueProcessor;
        private Func<string, LogLevel, bool> _filter;

        static TextBlockLogger()
        {
            var logLevelString = GetLogLevelString(LogLevel.Information);
            messagePadding = new string(' ', logLevelString.Length + loglevelPadding.Length);
            newLineWithMessagePadding = Environment.NewLine + messagePadding;
        }

        public TextBlockLogger(string name, Func<string, LogLevel, bool> filter, bool includeScopes, TextBlock textBlock)
            : this(name, filter, includeScopes ? new LoggerExternalScopeProvider() : null, new TextBlockLoggerProcessor(textBlock))
        {
        }

        internal TextBlockLogger(string name, Func<string, LogLevel, bool> filter, IExternalScopeProvider scopeProvider, TextBlockLoggerProcessor loggerProcessor)
        {
            queueProcessor = loggerProcessor ?? throw new ArgumentNullException(nameof(loggerProcessor));
            Name = name ?? throw new ArgumentNullException(nameof(name));

            Filter = filter ?? ((category, logLevel) => true);
            ScopeProvider = scopeProvider;
        }

        public bool DisableColors
        {
            get; set;
        }

        public Func<string, LogLevel, bool> Filter
        {
            get => _filter;
            set => _filter = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Name
        {
            get;
        }

        internal IExternalScopeProvider ScopeProvider
        {
            get;
            set;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? EmptyDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.None)
            {
                return false;
            }

            return Filter(Name, logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                WriteMessage(logLevel, Name, eventId.Id, message, exception);
            }
        }

        public virtual void WriteMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            var logBuilder = TextBlockLogger.logBuilder;
            TextBlockLogger.logBuilder = null;

            if (logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }

            var logLevelColors = default(LevelColors);
            var logLevelString = string.Empty;

            // Example:
            // INFO: ConsoleApp.Program[10]
            //       Request received

            logLevelColors = GetLogLevelColors(logLevel);
            logLevelString = GetLogLevelString(logLevel);
            // category and event id
            logBuilder.Append(loglevelPadding);
            logBuilder.Append(logName);
            logBuilder.Append("[");
            logBuilder.Append(eventId);
            logBuilder.AppendLine("]");

            // scope information
            GetScopeInformation(logBuilder);

            if (!string.IsNullOrEmpty(message))
            {
                // message
                logBuilder.Append(messagePadding);

                var len = logBuilder.Length;
                logBuilder.AppendLine(message);
                logBuilder.Replace(Environment.NewLine, newLineWithMessagePadding, len, message.Length);
            }

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                // exception message
                logBuilder.AppendLine(exception.ToString());
            }

            if (logBuilder.Length > 0)
            {
                var hasLevel = !string.IsNullOrEmpty(logLevelString);
                // Queue log message
                queueProcessor.EnqueueMessage(new LogMessageEntry()
                {
                    Message = logBuilder.ToString(),
                    MessageColor = DefaultTextBlockColor,
                    LevelString = hasLevel ? logLevelString : null,
                    LevelBackground = hasLevel ? logLevelColors.Background : null,
                    LevelForeground = hasLevel ? logLevelColors.Foreground : null
                });
            }

            logBuilder.Clear();
            if (logBuilder.Capacity > 1024)
            {
                logBuilder.Capacity = 1024;
            }
            TextBlockLogger.logBuilder = logBuilder;
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "TRACE";

                case LogLevel.Debug:
                    return "DEBUG";

                case LogLevel.Information:
                    return "INFO";

                case LogLevel.Warning:
                    return "WARN";

                case LogLevel.Error:
                    return "FAIL";

                case LogLevel.Critical:
                    return "CRIT";

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private LevelColors GetLogLevelColors(LogLevel logLevel)
        {
            if (DisableColors)
            {
                return new LevelColors(null, null);
            }

            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return new LevelColors(Brushes.Red, Brushes.White);

                case LogLevel.Error:
                    return new LevelColors(Brushes.Red, Brushes.Black);

                case LogLevel.Warning:
                    return new LevelColors(Brushes.Yellow, Brushes.Black);

                case LogLevel.Information:
                    return new LevelColors(Brushes.DarkGreen, Brushes.Black);

                case LogLevel.Debug:
                    return new LevelColors(Brushes.Gray, Brushes.Black);

                case LogLevel.Trace:
                    return new LevelColors(Brushes.Gray, Brushes.Black);

                default:
                    return new LevelColors(DefaultTextBlockColor, DefaultTextBlockColor);
            }
        }

        private void GetScopeInformation(StringBuilder stringBuilder)
        {
            var scopeProvider = ScopeProvider;
            if (scopeProvider != null)
            {
                var initialLength = stringBuilder.Length;

                scopeProvider.ForEachScope((scope, state) =>
                {
                    var (builder, length) = state;
                    var first = length == builder.Length;
                    builder.Append(first ? "=> " : " => ").Append(scope);
                }, (stringBuilder, initialLength));

                if (stringBuilder.Length > initialLength)
                {
                    stringBuilder.Insert(initialLength, messagePadding);
                    stringBuilder.AppendLine();
                }
            }
        }

        private struct LevelColors
        {
            public LevelColors(Brush foreground, Brush background)
            {
                Foreground = foreground;
                Background = background;
            }

            public Brush Background
            {
                get;
            }

            public Brush Foreground
            {
                get;
            }
        }

        private class EmptyDisposable : IDisposable
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static EmptyDisposable()
            {
                Instance = new EmptyDisposable();
            }

            private EmptyDisposable()
            {
            }

            public static EmptyDisposable Instance
            {
                get;
            }

            public void Dispose()
            {
            }
        }
    }
}