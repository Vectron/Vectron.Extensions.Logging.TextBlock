using System.Collections.Concurrent;
using System.Windows.Controls;
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
        => Dispatcher.CurrentDispatcher.Invoke(() =>
        {
            var closeMenuItem = new MenuItem()
            {
                Header = "Clear",
            };
            closeMenuItem.Click += (o, e) => textBlock.Inlines.Clear();
            textBlock.ContextMenu ??= new ContextMenu();
            _ = textBlock.ContextMenu.Items.Add(closeMenuItem);
            textBlock.Unloaded += TextBlock_Unloaded;
            _ = sinks.TryAdd(textBlock, new AnsiParsingLogTextBlock(textBlock, options.CurrentValue.MaxMessages));
        });

    /// <inheritdoc/>
    public void Dispose()
        => optionsReloadToken?.Dispose();

    /// <inheritdoc/>
    public void RemoveTextBlock(System.Windows.Controls.TextBlock textBlock)
        => _ = sinks.TryRemove(textBlock, out _);

    private void ReloadLoggerOptions(TextBlockLoggerOptions currentValue)
    {
        foreach (var sink in sinks)
        {
            sink.Value.MaxMessages = currentValue.MaxMessages;
        }
    }

    private void TextBlock_Unloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.TextBlock textBlock)
        {
            RemoveTextBlock(textBlock);
        }
    }
}
