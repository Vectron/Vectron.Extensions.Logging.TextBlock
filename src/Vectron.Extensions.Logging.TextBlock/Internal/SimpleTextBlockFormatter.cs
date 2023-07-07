using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Vectron.Extensions.Logging.TextBlock.Internal;

/// <summary>
/// A simple log message formatter.
/// </summary>
internal sealed class SimpleTextBlockFormatter : TextBlockFormatter, IDisposable
{
    private const string LogLevelPadding = ": ";
    private static readonly string MessagePadding = new(' ', GetLogLevelString(LogLevel.Information).Length + LogLevelPadding.Length);
    private static readonly string NewLineWithMessagePadding = Environment.NewLine + MessagePadding;
    private readonly IDisposable? optionsReloadToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleTextBlockFormatter"/> class.
    /// </summary>
    /// <param name="options">The options for this <see cref="SimpleTextBlockFormatter"/>.</param>
    public SimpleTextBlockFormatter(IOptionsMonitor<SimpleTextBlockFormatterOptions> options)
        : base(TextBlockFormatterNames.Simple)
    {
        ReloadLoggerOptions(options.CurrentValue);
        optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <summary>
    /// Gets or sets the options instance for this <see cref="SimpleTextBlockFormatter"/>.
    /// </summary>
    internal SimpleTextBlockFormatterOptions FormatterOptions
    {
        get;
        set;
    }

    /// <inheritdoc/>
    public void Dispose()
        => optionsReloadToken?.Dispose();

    /// <inheritdoc/>
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        if (logEntry.Formatter == null)
        {
            return;
        }

        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }

        var logLevel = logEntry.LogLevel;
        var logLevelColors = GetLogLevelColors(logLevel);
        var logLevelString = GetLogLevelString(logLevel);

        if (FormatterOptions.TimestampFormat != null)
        {
            var dateTimeOffset = GetCurrentDateTime();
            var timestamp = dateTimeOffset.ToString(FormatterOptions.TimestampFormat, CultureInfo.CurrentCulture);
            textWriter.Write(timestamp);
        }

        textWriter.WriteColoredMessage(logLevelString, logLevelColors.Background, logLevelColors.Foreground);
        CreateDefaultLogMessage(textWriter, logEntry, message, scopeProvider);
    }

    private static string GetLogLevelString(LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => "trace",
            LogLevel.Debug => "debug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            LogLevel.None => "none",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };

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
                textWriter.Write(MessagePadding);
                WriteReplacing(textWriter, Environment.NewLine, NewLineWithMessagePadding, message);
                textWriter.Write(Environment.NewLine);
            }
        }

        static void WriteReplacing(TextWriter writer, string oldValue, string newValue, string message)
        {
            var newMessage = message.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);
            writer.Write(newMessage);
        }
    }

    private void CreateDefaultLogMessage<TState>(TextWriter textWriter, LogEntry<TState> logEntry, string message, IExternalScopeProvider scopeProvider)
    {
        var singleLine = FormatterOptions.SingleLine;
        var eventId = logEntry.EventId.Id;
        var exception = logEntry.Exception;

        // Example:
        // info: WPFApp.Program[10] Request received

        // category and event id
        textWriter.Write(LogLevelPadding);
        textWriter.Write(logEntry.Category);
        textWriter.Write('[');

        Span<char> span = stackalloc char[10];
        if (eventId.TryFormat(span, out var charsWritten, default, CultureInfo.CurrentCulture))
        {
            textWriter.Write(span[..charsWritten]);
        }
        else
        {
            textWriter.Write(eventId.ToString(CultureInfo.CurrentCulture));
        }

        textWriter.Write(']');
        if (!singleLine)
        {
            textWriter.Write(Environment.NewLine);
        }

        // scope information
        WriteScopeInformation(textWriter, scopeProvider, singleLine);
        WriteMessage(textWriter, message, singleLine);

        // Example: System.InvalidOperationException at Namespace.Class.Function() in File:line X
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

    private DateTimeOffset GetCurrentDateTime()
        => FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;

    private TextBlockColors GetLogLevelColors(LogLevel logLevel)
    {
        var disableColors = FormatterOptions.ColorBehavior is LoggerColorBehavior.Disabled or LoggerColorBehavior.Default;
        if (disableColors)
        {
            return new TextBlockColors(Foreground: null, Background: null);
        }

        return logLevel switch
        {
            LogLevel.Trace => new TextBlockColors(ConsoleColor.Gray, Background: null),
            LogLevel.Debug => new TextBlockColors(ConsoleColor.Gray, Background: null),
            LogLevel.Information => new TextBlockColors(ConsoleColor.DarkGreen, Background: null),
            LogLevel.Warning => new TextBlockColors(ConsoleColor.Yellow, Background: null),
            LogLevel.Error => new TextBlockColors(ConsoleColor.Black, ConsoleColor.DarkRed),
            LogLevel.Critical => new TextBlockColors(ConsoleColor.White, ConsoleColor.DarkRed),
            LogLevel.None => new TextBlockColors(Foreground: null, Background: null),
            _ => new TextBlockColors(Foreground: null, Background: null),
        };
    }

    [MemberNotNull(nameof(FormatterOptions))]
    private void ReloadLoggerOptions(SimpleTextBlockFormatterOptions options)
        => FormatterOptions = options;

    private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider scopeProvider, bool singleLine)
    {
        if (FormatterOptions.IncludeScopes && scopeProvider != null)
        {
            var paddingNeeded = !singleLine;
            scopeProvider.ForEachScope(
                (scope, state) =>
                {
                    if (paddingNeeded)
                    {
                        paddingNeeded = false;
                        state.Write(MessagePadding);
                        state.Write("=> ");
                    }
                    else
                    {
                        state.Write(" => ");
                    }

                    state.Write(scope);
                },
                textWriter);

            if (!paddingNeeded && !singleLine)
            {
                textWriter.Write(Environment.NewLine);
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    private record struct TextBlockColors(ConsoleColor? Foreground, ConsoleColor? Background);
}
