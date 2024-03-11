using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Vectron.Extensions.Logging.TextBlock.Internal;

namespace Vectron.Extensions.Logging.TextBlock.Formatters;

/// <summary>
/// Simple implementation of the <see cref="TextBlockFormatter"/>.
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
    /// <param name="options">The formatter options.</param>
    public SimpleTextBlockFormatter(IOptionsMonitor<SimpleTextBlockFormatterOptions> options)
        : base(TextBlockFormatterNames.Simple)
    {
        ReloadLoggerOptions(options.CurrentValue);
        optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <summary>
    /// Gets or sets the options for this formatter.
    /// </summary>
    internal SimpleTextBlockFormatterOptions FormatterOptions
    {
        get;
        set;
    }

    // returns true on MacCatalyst
    private static bool IsAndroidOrAppleMobile => OperatingSystem.IsAndroid() || OperatingSystem.IsTvOS() || OperatingSystem.IsIOS();

    /// <inheritdoc/>
    public void Dispose()
        => optionsReloadToken?.Dispose();

    /// <inheritdoc/>
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }

        var logLevel = logEntry.LogLevel;
        var logLevelColors = GetLogLevelColors(logLevel);
        var logLevelString = GetLogLevelString(logLevel);

        string? timestamp = null;
        var timestampFormat = FormatterOptions.TimestampFormat;
        if (timestampFormat != null)
        {
            var dateTimeOffset = GetCurrentDateTime();
            timestamp = dateTimeOffset.ToString(timestampFormat, CultureInfo.CurrentCulture);
        }

        if (timestamp != null)
        {
            textWriter.Write(timestamp);
        }

        if (logLevelString != null)
        {
            textWriter.Write(logLevelColors + logLevelString + "\u001b[0m");
        }

        CreateDefaultLogMessage(textWriter, logEntry, message, scopeProvider);
    }

    private static string GetLogLevelString(LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            LogLevel.None => throw new NotSupportedException(),
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
            var newMessage = message.Replace(oldValue, newValue, StringComparison.Ordinal);
            writer.Write(newMessage);
        }
    }

    private void CreateDefaultLogMessage<TState>(TextWriter textWriter, in LogEntry<TState> logEntry, string message, IExternalScopeProvider? scopeProvider)
    {
        var singleLine = FormatterOptions.SingleLine;
        var eventId = logEntry.EventId.Id;
        var exception = logEntry.Exception;

        // Example:
        // info: App.Program[10]
        //       Request received

        // category and event id
        textWriter.Write(LogLevelPadding);
        textWriter.Write(logEntry.Category);
        textWriter.Write('[');

        Span<char> span = stackalloc char[10];
        if (eventId.TryFormat(span, out var charsWritten, provider: CultureInfo.CurrentCulture))
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

    private string GetLogLevelColors(LogLevel logLevel)
    {
        // We shouldn't be outputting color codes for Android/Apple mobile platforms,
        // they have no shell (adb shell is not meant for running apps) and all the output gets redirected to some log file.
        var disableColors = (FormatterOptions.ColorBehavior == LoggerColorBehavior.Disabled) ||
            (FormatterOptions.ColorBehavior == LoggerColorBehavior.Default && IsAndroidOrAppleMobile);
        if (disableColors)
        {
            return string.Empty;
        }

        // We must explicitly set the background color if we are setting the foreground color,
        // since just setting one can look bad.
        return logLevel switch
        {
            LogLevel.Trace => "\x1b[37m",
            LogLevel.Debug => "\x1b[37m",
            LogLevel.Information => "\x1b[32m",
            LogLevel.Warning => "\x1b[1m\x1b[33m",
            LogLevel.Error => "\x1b[30m\x1b[41m",
            LogLevel.Critical => "\x1b[1m\x1b[37m\x1b[41m",
            LogLevel.None => string.Empty,
            _ => string.Empty,
        };
    }

    [MemberNotNull(nameof(FormatterOptions))]
    private void ReloadLoggerOptions(SimpleTextBlockFormatterOptions options)
        => FormatterOptions = options;

    private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider? scopeProvider, bool singleLine)
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
}
