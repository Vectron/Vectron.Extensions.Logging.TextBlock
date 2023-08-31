using System.Windows.Documents;
using System.Windows.Media;
using Vectron.Ansi;

namespace Vectron.Extensions.Logging.TextBlock.Internal;

/// <summary>
/// An ANSI string parser that writes to a <see cref="TextBlock"/>.
/// </summary>
internal sealed class AnsiParsingTextBlock : ITextBlock
{
    private readonly AnsiParser parser;
    private readonly System.Windows.Controls.TextBlock textBlock;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiParsingTextBlock"/> class.
    /// </summary>
    /// <param name="textBlock">The textBlock we are writing to.</param>
    /// <param name="maxMessages">Maximum number of messages to display in the <see cref="TextBlock"/>.</param>
    public AnsiParsingTextBlock(System.Windows.Controls.TextBlock textBlock, int maxMessages)
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

    private void WriteToTextBlock(ReadOnlySpan<char> text, AnsiParserFoundStyle parsedStyle, IEnumerable<string> unknownCodes)
    {
        var textString = text.ToString();
        textBlock.Dispatcher.Invoke(() =>
            {
                var (foregroundRed, foregroundGreen, foregroundBlue) = parsedStyle.ConvertForegroundColorToRGB();
                var foregroundBrush = parsedStyle.HasForegroundColor
                    ? new SolidColorBrush(Color.FromRgb((byte)foregroundRed, (byte)foregroundGreen, (byte)foregroundBlue))
                    : Brushes.Black;

                var (backgroundRed, backgroundGreen, backgroundBlue) = parsedStyle.ConvertBackgroundColorToRGB();
                var backgroundBrush = parsedStyle.HasBackgroundColor
                    ? new SolidColorBrush(Color.FromRgb((byte)backgroundRed, (byte)backgroundGreen, (byte)backgroundBlue))
                    : null;

                var run = new Run(textString)
                {
                    Background = backgroundBrush,
                    Foreground = foregroundBrush,
                };

                textBlock.Inlines.Add(run);

                if (textBlock.Inlines.Count > MaxMessages)
                {
                    _ = textBlock.Inlines.Remove(textBlock.Inlines.FirstInline);
                }
            });
    }
}
