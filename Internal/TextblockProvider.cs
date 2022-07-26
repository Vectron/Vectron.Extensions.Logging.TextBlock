using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Extensions.Options;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// default implementation of <see cref="ITextblockProvider"/>.
/// </summary>
internal class TextblockProvider : ITextblockProvider, IDisposable
{
    private readonly MenuItem closeMenuItem = new()
    {
        Header = "Clear",
    };

    private readonly IOptionsMonitor<TextBlockLoggerOptions> options;
    private readonly IDisposable optionsReloadToken;
    private readonly ConcurrentDictionary<TextBlock, ITextBlock> sinks = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TextblockProvider"/> class.
    /// </summary>
    /// <param name="options">The <see cref="TextBlockLoggerOptions"/> monitor.</param>
    public TextblockProvider(IOptionsMonitor<TextBlockLoggerOptions> options)
    {
        ReloadLoggerOptions(options.CurrentValue);
        optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        this.options = options;
    }

    /// <inheritdoc/>
    public IEnumerable<ITextBlock> Sinks => sinks.Values;

    /// <inheritdoc/>
    public void AddTextBlock(TextBlock textBlock)
    {
        closeMenuItem.Click += (o, e) => textBlock.Inlines.Clear();

        if (textBlock.ContextMenu == null)
        {
            textBlock.ContextMenu = new ContextMenu();
        }

        _ = textBlock.ContextMenu.Items.Add(closeMenuItem);

        textBlock.Unloaded += TextBlock_Unloaded;
        _ = sinks.TryAdd(textBlock, new AnsiParsingLogTextblock(textBlock, options.CurrentValue.MaxMessages));
    }

    /// <inheritdoc/>
    public void Dispose()
        => optionsReloadToken.Dispose();

    /// <inheritdoc/>
    public void RemoveTextBlock(TextBlock textblock)
        => _ = sinks.TryRemove(textblock, out _);

    private void ReloadLoggerOptions(TextBlockLoggerOptions currentValue)
    {
        foreach (var sink in sinks)
        {
            sink.Value.MaxMessages = currentValue.MaxMessages;
        }
    }

    private void TextBlock_Unloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (sender is TextBlock textBlock)
        {
            RemoveTextBlock(textBlock);
        }
    }
}