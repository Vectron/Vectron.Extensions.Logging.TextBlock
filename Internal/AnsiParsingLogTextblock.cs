using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// An ansi string parser that writes to a <see cref="TextBlock"/>.
/// </summary>
internal class AnsiParsingLogTextBlock : ITextBlock
{
    private readonly AnsiParser parser;
    private readonly TextBlock textBlock;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiParsingLogTextBlock"/> class.
    /// </summary>
    /// <param name="textBlock">The textBlock we are writing to.</param>
    /// <param name="maxMessages">Maximum number of messages to display in the <see cref="TextBlock"/>.</param>
    public AnsiParsingLogTextBlock(TextBlock textBlock, int maxMessages)
    {
        this.textBlock = textBlock;
        MaxMessages = maxMessages;
        parser = new AnsiParser(WriteToTextBlock);
    }

    /// <inheritdoc/>
    public int MaxMessages
    {
        get;
        set;
    }

    /// <inheritdoc/>
    public void Write(string message)
        => parser.Parse(message);

    private static Brush ConsoleColorToBrush(ConsoleColor color)
        => color switch
        {
            ConsoleColor.Black => new SolidColorBrush(Color.FromRgb(12, 12, 12)),
            ConsoleColor.DarkBlue => new SolidColorBrush(Color.FromRgb(0, 55, 218)),
            ConsoleColor.DarkGreen => new SolidColorBrush(Color.FromRgb(19, 161, 14)),
            ConsoleColor.DarkCyan => new SolidColorBrush(Color.FromRgb(58, 150, 221)),
            ConsoleColor.DarkRed => new SolidColorBrush(Color.FromRgb(197, 15, 31)),
            ConsoleColor.DarkMagenta => new SolidColorBrush(Color.FromRgb(136, 23, 152)),
            ConsoleColor.DarkYellow => new SolidColorBrush(Color.FromRgb(193, 156, 0)),
            ConsoleColor.Gray => new SolidColorBrush(Color.FromRgb(204, 204, 204)),
            ConsoleColor.DarkGray => new SolidColorBrush(Color.FromRgb(118, 118, 118)),
            ConsoleColor.Blue => new SolidColorBrush(Color.FromRgb(59, 120, 255)),
            ConsoleColor.Green => new SolidColorBrush(Color.FromRgb(22, 198, 12)),
            ConsoleColor.Cyan => new SolidColorBrush(Color.FromRgb(97, 214, 214)),
            ConsoleColor.Red => new SolidColorBrush(Color.FromRgb(231, 72, 86)),
            ConsoleColor.Magenta => new SolidColorBrush(Color.FromRgb(180, 0, 158)),
            ConsoleColor.Yellow => new SolidColorBrush(Color.FromRgb(249, 241, 165)),
            ConsoleColor.White => new SolidColorBrush(Color.FromRgb(242, 242, 242)),
            _ => Brushes.Black,
        };

    private void WriteToTextBlock(string message, int startIndex, int length, ConsoleColor? background, ConsoleColor? foreground)
        => textBlock.Dispatcher.Invoke(() =>
        {
            var span = message.AsSpan(startIndex, length);
            var run = new Run(span.ToString());
            if (background.HasValue)
            {
                run.Background = ConsoleColorToBrush(background.Value);
            }

            if (foreground.HasValue)
            {
                run.Foreground = ConsoleColorToBrush(foreground.Value);
            }

            textBlock.Inlines.Add(run);

            if (textBlock.Inlines.Count > MaxMessages)
            {
                _ = textBlock.Inlines.Remove(textBlock.Inlines.FirstInline);
            }
        });
}