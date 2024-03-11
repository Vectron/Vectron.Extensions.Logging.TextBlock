using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Vectron.Extensions.Logging.TextBlock.Internal;

namespace Vectron.Extensions.Logging.TextBlock.Formatters;

/// <summary>
/// System d implementation of the <see cref="TextBlockFormatter"/>.
/// </summary>
internal sealed class SystemdTextBlockFormatter : TextBlockFormatter, IDisposable
{
    private readonly IDisposable? optionsReloadToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemdTextBlockFormatter"/> class.
    /// </summary>
    /// <param name="options">The formatter options.</param>
    public SystemdTextBlockFormatter(IOptionsMonitor<TextBlockFormatterOptions> options)
        : base(TextBlockFormatterNames.Systemd)
    {
        ReloadLoggerOptions(options.CurrentValue);
        optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <summary>
    /// Gets or sets the options for this formatter.
    /// </summary>
    internal TextBlockFormatterOptions FormatterOptions
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

        var logLevel = logEntry.LogLevel;
        var category = logEntry.Category;
        var eventId = logEntry.EventId.Id;
        var exception = logEntry.Exception;

        // <6>App.Program[10] Request received
        // log level
        var logLevelString = GetSyslogSeverityString(logLevel);
        textWriter.Write(logLevelString);

        // timestamp
        var timestampFormat = FormatterOptions.TimestampFormat;
        if (timestampFormat != null)
        {
            var dateTimeOffset = GetCurrentDateTime();
            textWriter.Write(dateTimeOffset.ToString(timestampFormat, CultureInfo.CurrentCulture));
        }

        // category and event id
        textWriter.Write(category);
        textWriter.Write('[');
        textWriter.Write(eventId);
        textWriter.Write(']');

        // scope information
        WriteScopeInformation(textWriter, scopeProvider);

        // message
        if (!string.IsNullOrEmpty(message))
        {
            textWriter.Write(' ');
            WriteReplacingNewLine(textWriter, message);
        }

        // exception
        // System.InvalidOperationException at Namespace.Class.Function() in File:line X
        if (exception != null)
        {
            textWriter.Write(' ');
            WriteReplacingNewLine(textWriter, exception.ToString());
        }

        // newline delimiter
        textWriter.Write(Environment.NewLine);

        static void WriteReplacingNewLine(TextWriter writer, string message)
        {
            var newMessage = message.Replace(Environment.NewLine, " ", StringComparison.Ordinal);
            writer.Write(newMessage);
        }
    }

    // 'Syslog Message Severities' from https://tools.ietf.org/html/rfc5424.
    private static string GetSyslogSeverityString(LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => "<7>",
            LogLevel.Debug => "<7>",        // debug-level messages
            LogLevel.Information => "<6>",  // informational messages
            LogLevel.Warning => "<4>",     // warning conditions
            LogLevel.Error => "<3>",       // error conditions
            LogLevel.Critical => "<2>",    // critical conditions
            LogLevel.None => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };

    private DateTimeOffset GetCurrentDateTime()
        => FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;

    [MemberNotNull(nameof(FormatterOptions))]
    private void ReloadLoggerOptions(TextBlockFormatterOptions options)
        => FormatterOptions = options;

    private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider? scopeProvider)
    {
        if (FormatterOptions.IncludeScopes && scopeProvider != null)
        {
            scopeProvider.ForEachScope(
                (scope, state) =>
                {
                    state.Write(" => ");
                    state.Write(scope);
                },
                textWriter);
        }
    }
}
