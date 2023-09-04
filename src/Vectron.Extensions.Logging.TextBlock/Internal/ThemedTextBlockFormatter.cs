using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Vectron.Ansi;

namespace Vectron.Extensions.Logging.TextBlock.Internal;

/// <summary>
/// A <see cref="TextBlockFormatter"/> for writing messages as a single line.
/// </summary>
internal sealed class ThemedTextBlockFormatter : TextBlockFormatter, IDisposable
{
    private const string InnerExceptionPrefix = " ---> ";
    private const string LogLevelPadding = ": ";
    private static readonly string MessagePadding = new(' ', GetMaximumLogLevelLength() + LogLevelPadding.Length);
    private static readonly string NewLineWithMessagePadding = Environment.NewLine + MessagePadding;
    private readonly IDisposable? optionsReloadToken;
    private readonly IThemeProvider themeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemedTextBlockFormatter"/> class.
    /// </summary>
    /// <param name="options">Options for setting up this formatter.</param>
    /// <param name="themeProvider">The registered themes.</param>
    public ThemedTextBlockFormatter(IOptionsMonitor<ThemedTextBlockFormatterOptions> options, IThemeProvider themeProvider)
        : base(TextBlockFormatterNames.Themed)
    {
        this.themeProvider = themeProvider;
        ReloadLoggerOptions(options.CurrentValue);
        optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <summary>
    /// Gets or sets the formatter options.
    /// </summary>
    internal ThemedTextBlockFormatterOptions FormatterOptions
    {
        get;
        set;
    }

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

        if (FormatterOptions.ColorWholeLine)
        {
            var lineColor = themeProvider.GetLineColor(logEntry.LogLevel);
            textWriter.Write(lineColor);
        }

        WriteTime(textWriter);
        WriteLogLevel(logEntry.LogLevel, textWriter);
        WriteCategory(logEntry.Category, textWriter);
        WriteEventId(logEntry.EventId, textWriter);
        WriteScopeInformation(textWriter, scopeProvider);
        WriteLogMessage(textWriter, message);
        WriteException(textWriter, logEntry.Exception);
        if (FormatterOptions.ColorWholeLine)
        {
            textWriter.WriteResetColorAndStyle();
        }

        textWriter.WriteLine();
    }

    private static string GetLogLevelString(LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => "TRACE",
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "FAIL",
            LogLevel.Critical => "CRIT",
            LogLevel.None => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };

    private static int GetMaximumLogLevelLength()
    {
        var length = 0;
        foreach (LogLevel level in Enum.GetValues(typeof(LogLevel)))
        {
            try
            {
                var levelString = GetLogLevelString(level);
                if (levelString.Length > length)
                {
                    length = levelString.Length;
                }
            }
            catch
            {
            }
        }

        return length;
    }

    private DateTimeOffset GetCurrentDateTime()
        => FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;

    [MemberNotNull(nameof(FormatterOptions))]
    private void ReloadLoggerOptions(ThemedTextBlockFormatterOptions options) => FormatterOptions = options;

    private void WriteCategory(string category, TextWriter textWriter)
    {
        if (string.IsNullOrEmpty(category))
        {
            return;
        }

        if (FormatterOptions.ColorWholeLine)
        {
            textWriter.Write(category);
            return;
        }

        var color = themeProvider.GetCategoryColor(category);
        textWriter.WriteEscaped(color, category);
    }

    private void WriteEventId(EventId eventId, TextWriter textWriter)
    {
        var color = themeProvider.GetEventIdColor(eventId);
        if (!FormatterOptions.ColorWholeLine && !string.IsNullOrEmpty(color))
        {
            textWriter.Write(color);
        }

        textWriter.Write('[');
        Span<char> span = stackalloc char[10];

        if (eventId.Id.TryFormat(span, out var charsWritten, default, CultureInfo.CurrentCulture))
        {
            textWriter.Write(span[..charsWritten]);
        }
        else
        {
            var id = eventId.Id.ToString(CultureInfo.CurrentCulture);
            textWriter.Write(id);
        }

        textWriter.Write(']');

        if (!FormatterOptions.ColorWholeLine && !string.IsNullOrEmpty(color))
        {
            textWriter.WriteResetColorAndStyle();
        }

        textWriter.Write(' ');
    }

    private void WriteException(TextWriter textWriter, Exception? exception)
    {
        if (exception == null)
        {
            return;
        }

        textWriter.Write(NewLineWithMessagePadding);
        var message = exception.ToString().Replace(Environment.NewLine, NewLineWithMessagePadding, StringComparison.Ordinal);

        if (FormatterOptions.ColorWholeLine)
        {
            textWriter.Write(message);
            return;
        }

        var color = themeProvider.GetExceptionColor(exception);
        textWriter.WriteEscaped(color, message);
    }

    private void WriteLogLevel(LogLevel logLevel, TextWriter textWriter)
    {
        var logLevelString = GetLogLevelString(logLevel);
        if (FormatterOptions.ColorWholeLine)
        {
            textWriter.Write(logLevelString);
        }
        else
        {
            var color = themeProvider.GetLogLevelColor(logLevel);
            textWriter.WriteEscaped(color, logLevelString);
        }

        textWriter.Write(LogLevelPadding);
    }

    private void WriteLogMessage(TextWriter textWriter, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        var newMessage = message.Replace(Environment.NewLine, NewLineWithMessagePadding, StringComparison.Ordinal);
        if (FormatterOptions.ColorWholeLine)
        {
            textWriter.Write(newMessage);
            return;
        }

        var color = themeProvider.GetMessageColor(newMessage);
        textWriter.WriteEscaped(color, newMessage);
    }

    private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider? scopeProvider)
    {
        if (!FormatterOptions.IncludeScopes || scopeProvider == null)
        {
            return;
        }

        scopeProvider.ForEachScope(
            (scope, state) =>
            {
                var color = themeProvider.GetScopeColor(scope);
                state.Write("=> ");

                if (FormatterOptions.ColorWholeLine)
                {
                    textWriter.Write(scope);
                }
                else
                {
                    textWriter.WriteEscaped(color, scope?.ToString() ?? string.Empty);
                }

                textWriter.Write(' ');
            },
            textWriter);
    }

    private void WriteTime(TextWriter textWriter)
    {
        var timestampFormat = FormatterOptions.TimestampFormat;
        if (string.IsNullOrEmpty(timestampFormat))
        {
            return;
        }

        var dateTimeOffset = GetCurrentDateTime();
        var timestamp = dateTimeOffset.ToString(timestampFormat, CultureInfo.CurrentCulture);

        if (FormatterOptions.ColorWholeLine)
        {
            textWriter.Write(timestamp);
        }
        else
        {
            var color = themeProvider.GetTimeColor(dateTimeOffset);
            textWriter.WriteEscaped(color, timestamp);
        }

        textWriter.Write(' ');
    }
}
