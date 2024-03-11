using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Vectron.Extensions.Logging.TextBlock.Internal;

namespace Vectron.Extensions.Logging.TextBlock.Formatters;

/// <summary>
/// Json implementation of the <see cref="TextBlockFormatter"/>.
/// </summary>
internal sealed class JsonTextBlockFormatter : TextBlockFormatter, IDisposable
{
    private readonly IDisposable? optionsReloadToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonTextBlockFormatter"/> class.
    /// </summary>
    /// <param name="options">The formatter options.</param>
    public JsonTextBlockFormatter(IOptionsMonitor<JsonTextBlockFormatterOptions> options)
        : base(TextBlockFormatterNames.Json)
    {
        ReloadLoggerOptions(options.CurrentValue);
        optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <summary>
    /// Gets or sets the options for this formatter.
    /// </summary>
    internal JsonTextBlockFormatterOptions FormatterOptions
    {
        get; set;
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
        const int defaultBufferSize = 1024;

        var output = new ArrayBufferWriter<byte>(defaultBufferSize);
        using (var writer = new Utf8JsonWriter(output, FormatterOptions.JsonWriterOptions))
        {
            writer.WriteStartObject();
            var timestampFormat = FormatterOptions.TimestampFormat;
            if (timestampFormat != null)
            {
                var dateTimeOffset = FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
                writer.WriteString("Timestamp", dateTimeOffset.ToString(timestampFormat, CultureInfo.CurrentCulture));
            }

            writer.WriteNumber(nameof(logEntry.EventId), eventId);
            writer.WriteString(nameof(logEntry.LogLevel), GetLogLevelString(logLevel));
            writer.WriteString(nameof(logEntry.Category), category);
            writer.WriteString("Message", message);

            if (exception != null)
            {
                writer.WriteString(nameof(Exception), exception.ToString());
            }

            if (logEntry.State != null)
            {
                writer.WriteStartObject(nameof(logEntry.State));
                writer.WriteString("Message", logEntry.State.ToString());
                if (logEntry.State is IReadOnlyCollection<KeyValuePair<string, object>> stateProperties)
                {
                    foreach (var item in stateProperties)
                    {
                        WriteItem(writer, item);
                    }
                }

                writer.WriteEndObject();
            }

            WriteScopeInformation(writer, scopeProvider);
            writer.WriteEndObject();
            writer.Flush();
        }

        textWriter.Write(Encoding.UTF8.GetString(output.WrittenMemory.Span));

        textWriter.Write(Environment.NewLine);
    }

    private static string GetLogLevelString(LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => "Trace",
            LogLevel.Debug => "Debug",
            LogLevel.Information => "Information",
            LogLevel.Warning => "Warning",
            LogLevel.Error => "Error",
            LogLevel.Critical => "Critical",
            LogLevel.None => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };

    private static string? ToInvariantString(object? obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "A lot of switch cases.")]
    private static void WriteItem(Utf8JsonWriter writer, KeyValuePair<string, object> item)
    {
        var key = item.Key;
        switch (item.Value)
        {
            case bool boolValue:
                writer.WriteBoolean(key, boolValue);
                break;

            case byte byteValue:
                writer.WriteNumber(key, byteValue);
                break;

            case sbyte sbyteValue:
                writer.WriteNumber(key, sbyteValue);
                break;

            case char charValue:
                writer.WriteString(key, MemoryMarshal.CreateSpan(ref charValue, 1));
                break;

            case decimal decimalValue:
                writer.WriteNumber(key, decimalValue);
                break;

            case double doubleValue:
                writer.WriteNumber(key, doubleValue);
                break;

            case float floatValue:
                writer.WriteNumber(key, floatValue);
                break;

            case int intValue:
                writer.WriteNumber(key, intValue);
                break;

            case uint uintValue:
                writer.WriteNumber(key, uintValue);
                break;

            case long longValue:
                writer.WriteNumber(key, longValue);
                break;

            case ulong ulongValue:
                writer.WriteNumber(key, ulongValue);
                break;

            case short shortValue:
                writer.WriteNumber(key, shortValue);
                break;

            case ushort ushortValue:
                writer.WriteNumber(key, ushortValue);
                break;

            case null:
                writer.WriteNull(key);
                break;

            default:
                writer.WriteString(key, ToInvariantString(item.Value));
                break;
        }
    }

    [MemberNotNull(nameof(FormatterOptions))]
    private void ReloadLoggerOptions(JsonTextBlockFormatterOptions options)
        => FormatterOptions = options;

    private void WriteScopeInformation(Utf8JsonWriter writer, IExternalScopeProvider? scopeProvider)
    {
        if (FormatterOptions.IncludeScopes && scopeProvider != null)
        {
            writer.WriteStartArray("Scopes");
            scopeProvider.ForEachScope(
                (scope, state) =>
                {
                    if (scope is IEnumerable<KeyValuePair<string, object>> scopeItems)
                    {
                        state.WriteStartObject();
                        state.WriteString("Message", scope.ToString());
                        foreach (var item in scopeItems)
                        {
                            WriteItem(state, item);
                        }

                        state.WriteEndObject();
                    }
                    else
                    {
                        state.WriteStringValue(ToInvariantString(scope));
                    }
                },
                writer);
            writer.WriteEndArray();
        }
    }
}
