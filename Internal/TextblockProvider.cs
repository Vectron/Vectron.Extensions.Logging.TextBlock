using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Controls;

namespace VectronsLibrary.TextBlockLogger
{
    /// <summary>
    /// default implementation of <see cref="ITextblockProvider"/>.
    /// </summary>
    internal class TextblockProvider : ITextblockProvider
    {
        private readonly MenuItem closeMenuItem = new MenuItem() { Header = "Clear" };
        private readonly ConcurrentDictionary<TextBlock, TextBlock> sinks = new ConcurrentDictionary<TextBlock, TextBlock>();

        /// <inheritdoc/>
        public IEnumerable<TextBlock> Sinks => sinks.Values;

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
            _ = sinks.TryAdd(textBlock, textBlock);
        }

        /// <inheritdoc/>
        public void RemoveTextBlock(TextBlock textblock)
        {
            _ = sinks.TryRemove(textblock, out _);
        }

        private void TextBlock_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                RemoveTextBlock(textBlock);
            }
        }
    }
}