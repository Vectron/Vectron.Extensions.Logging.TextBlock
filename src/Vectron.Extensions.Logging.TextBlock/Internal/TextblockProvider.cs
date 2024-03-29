using System.Collections.Concurrent;
using System.Windows.Threading;
using Microsoft.Extensions.Options;

namespace Vectron.Extensions.Logging.TextBlock.Internal;

/// <summary>
/// default implementation of <see cref="ITextBlockProvider"/>.
/// </summary>
internal sealed class TextBlockProvider : ITextBlockProvider, IDisposable
{
    private readonly IOptionsMonitor<TextBlockLoggerOptions> options;
    private readonly IDisposable? optionsReloadToken;
    private readonly ConcurrentDictionary<System.Windows.Controls.TextBlock, ITextBlock> sinks = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBlockProvider"/> class.
    /// </summary>
    /// <param name="options">The <see cref="TextBlockLoggerOptions"/> monitor.</param>
    public TextBlockProvider(IOptionsMonitor<TextBlockLoggerOptions> options)
    {
        ReloadLoggerOptions(options.CurrentValue);
        optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        this.options = options;
    }

    /// <inheritdoc/>
    public IEnumerable<ITextBlock> Sinks => sinks.Values;

    /// <inheritdoc/>
    public void AddTextBlock(System.Windows.Controls.TextBlock textBlock)
    {
        _ = sinks.TryAdd(textBlock, new AnsiParsingTextBlock(textBlock, options.CurrentValue.MaxMessages));
        textBlock.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
    }

    /// <inheritdoc/>
    public void Dispose()
        => optionsReloadToken?.Dispose();

    /// <inheritdoc/>
    public void RemoveTextBlock(System.Windows.Controls.TextBlock textBlock)
    {
        textBlock.Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
        _ = sinks.TryRemove(textBlock, out _);
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        if (sender is System.Windows.Controls.TextBlock textBlock)
        {
            RemoveTextBlock(textBlock);
            return;
        }

        if (sender is Dispatcher dispatcher)
        {
            var sinksToRemove = sinks.Keys.Where(x => x.Dispatcher == dispatcher).ToArray();
            foreach (var sink in sinksToRemove)
            {
                RemoveTextBlock(sink);
            }

            return;
        }
    }

    private void ReloadLoggerOptions(TextBlockLoggerOptions currentValue)
    {
        foreach (var sink in sinks)
        {
            sink.Value.MaxMessages = currentValue.MaxMessages;
        }
    }
}
