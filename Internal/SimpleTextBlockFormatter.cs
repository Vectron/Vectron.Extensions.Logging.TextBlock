using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Windows.Media;

namespace VectronsLibrary.TextBlockLogger
{
    internal class SimpleTextBlockFormatter : TextBlockFormatter, IDisposable
    {
        internal const string DefaultName = "Simple";
        private const string LoglevelPadding = ": ";
        private static readonly string messagePadding = new string(' ', GetLogLevelString(LogLevel.Information).Length + LoglevelPadding.Length);
        private static readonly string newLineWithMessagePadding = Environment.NewLine + messagePadding;
        private readonly IDisposable optionsReloadToken;

        public SimpleTextBlockFormatter(IOptionsMonitor<SimpleTextBlockFormatterOptions> options)
            : base(DefaultName)
        {
            FormatterOptions = new SimpleTextBlockFormatterOptions();
            ReloadLoggerOptions(options.CurrentValue);
            optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        }

        public SimpleTextBlockFormatterOptions FormatterOptions
        {
            get;
            set;
        }

        public void Dispose()
        {
            optionsReloadToken?.Dispose();
        }

        public override (string logLevelString, LevelColors logLevelColors) LogLevelData<TState>(in LogEntry<TState> logEntry)
        {
            LogLevel logLevel = logEntry.LogLevel;
            LevelColors logLevelColors = GetLogLevelColors(logLevel);
            string logLevelString = GetLogLevelString(logLevel);

            return (logLevelString, logLevelColors);
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (logEntry.Exception == null && message == null)
            {
                return;
            }
            string? timestamp = null;
            if (FormatterOptions.TimestampFormat != null)
            {
                DateTimeOffset dateTimeOffset = GetCurrentDateTime();
                timestamp = dateTimeOffset.ToString(FormatterOptions.TimestampFormat);
            }

            if (timestamp != null)
            {
                textWriter.Write(timestamp);
            }
            CreateDefaultLogMessage(textWriter, logEntry, message, scopeProvider);
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "TRACE",
                LogLevel.Debug => "DEBUG",
                LogLevel.Information => "INFO",
                LogLevel.Warning => "WARN",
                LogLevel.Error => "FAIL",
                LogLevel.Critical => "CRIT",
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
            };
        }

        private static void WriteMessage(TextWriter textWriter, string message, bool singleLine)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (singleLine)
                {
                    textWriter.Write(' ');
                    WriteReplacing(textWriter, Environment.NewLine, " ", message);
                }
                else
                {
                    textWriter.Write(messagePadding);
                    WriteReplacing(textWriter, Environment.NewLine, newLineWithMessagePadding, message);
                    textWriter.Write(Environment.NewLine);
                }
            }

            static void WriteReplacing(TextWriter writer, string oldValue, string newValue, string message)
            {
                string newMessage = message.Replace(oldValue, newValue);
                writer.Write(newMessage);
            }
        }

        private void CreateDefaultLogMessage<TState>(TextWriter textWriter, LogEntry<TState> logEntry, string message, IExternalScopeProvider scopeProvider)
        {
            bool singleLine = FormatterOptions.SingleLine;
            int eventId = logEntry.EventId.Id;
            Exception exception = logEntry.Exception;

            // Example:
            // info: WPFApp.Program[10]
            //       Request received

            // category and event id
            textWriter.Write(LoglevelPadding);
            textWriter.Write(logEntry.Category);
            textWriter.Write('[');

#if NETCOREAPP
            Span<char> span = stackalloc char[10];
            if (eventId.TryFormat(span, out int charsWritten))
                textWriter.Write(span.Slice(0, charsWritten));
            else
#endif
            textWriter.Write(eventId.ToString());

            textWriter.Write(']');
            if (!singleLine)
            {
                textWriter.Write(Environment.NewLine);
            }

            // scope information
            WriteScopeInformation(textWriter, scopeProvider, singleLine);
            WriteMessage(textWriter, message, singleLine);

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                // exception message
                WriteMessage(textWriter, exception.ToString(), singleLine);
            }
            if (singleLine)
            {
                textWriter.Write(Environment.NewLine);
            }
        }

        private DateTimeOffset GetCurrentDateTime() => FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;

        private LevelColors GetLogLevelColors(LogLevel logLevel)
        {
            if (FormatterOptions.DisableColors)
            {
                return new LevelColors(null, null);
            }

            return logLevel switch
            {
                LogLevel.Trace => new LevelColors(Brushes.Gray, Brushes.Black),
                LogLevel.Debug => new LevelColors(Brushes.Gray, Brushes.Black),
                LogLevel.Information => new LevelColors(Brushes.DarkGreen, Brushes.Black),
                LogLevel.Warning => new LevelColors(Brushes.Yellow, Brushes.Black),
                LogLevel.Error => new LevelColors(Brushes.Black, Brushes.DarkRed),
                LogLevel.Critical => new LevelColors(Brushes.White, Brushes.DarkRed),
                _ => new LevelColors(null, null)
            };
        }

        private void ReloadLoggerOptions(SimpleTextBlockFormatterOptions options)
        {
            FormatterOptions = options;
        }

        private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider scopeProvider, bool singleLine)
        {
            if (FormatterOptions.IncludeScopes && scopeProvider != null)
            {
                bool paddingNeeded = !singleLine;
                scopeProvider.ForEachScope((scope, state) =>
                {
                    if (paddingNeeded)
                    {
                        paddingNeeded = false;
                        state.Write(messagePadding);
                        state.Write("=> ");
                    }
                    else
                    {
                        state.Write(" => ");
                    }
                    state.Write(scope);
                }, textWriter);

                if (!paddingNeeded && !singleLine)
                {
                    textWriter.Write(Environment.NewLine);
                }
            }
        }
    }
}