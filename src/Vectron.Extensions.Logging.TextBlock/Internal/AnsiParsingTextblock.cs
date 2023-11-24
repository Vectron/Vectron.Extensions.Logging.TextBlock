using System.Windows.Documents;
using System.Windows.Media;
using Vectron.Ansi;

namespace Vectron.Extensions.Logging.TextBlock.Internal;

/// <summary>
/// An ANSI string parser that writes to a <see cref="TextBlock"/>.
/// </summary>
internal sealed class AnsiParsingTextBlock : ITextBlock
{
    private readonly System.Windows.Controls.TextBlock textBlock;
    private int lines;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiParsingTextBlock"/> class.
    /// </summary>
    /// <param name="textBlock">The textBlock we are writing to.</param>
    /// <param name="maxMessages">Maximum number of messages to display in the <see cref="TextBlock"/>.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Make the property look strange.")]
    public AnsiParsingTextBlock(System.Windows.Controls.TextBlock textBlock, int maxMessages)
    {
        this.textBlock = textBlock;
        MaxMessages = maxMessages;
    }

    /// <inheritdoc/>
    public int MaxMessages
    {
        get;
        set;
    }

    /// <inheritdoc/>
    public void Write(string message)
    {
        var parts = new Queue<MessagePart>();
        var parser = new AnsiParser(Append);
        parser.Parse(message);
        textBlock.Dispatcher.Invoke(() => AddToTextBox(parts), System.Windows.Threading.DispatcherPriority.ApplicationIdle);

        void Append(ReadOnlySpan<char> text, AnsiParserFoundStyle parsedStyle, IEnumerable<string> unknownCodes)
        {
            var textString = text.ToString();
            var (foregroundRed, foregroundGreen, foregroundBlue) = parsedStyle.ConvertForegroundColorToRGB();
            Color? foregroundColor = parsedStyle.HasForegroundColor
                ? Color.FromRgb((byte)foregroundRed, (byte)foregroundGreen, (byte)foregroundBlue)
                : null;

            var (backgroundRed, backgroundGreen, backgroundBlue) = parsedStyle.ConvertBackgroundColorToRGB();
            Color? backgroundBrush = parsedStyle.HasBackgroundColor
                ? Color.FromRgb((byte)backgroundRed, (byte)backgroundGreen, (byte)backgroundBlue)
                : null;

            var messagePart = new MessagePart(textString, foregroundColor, backgroundBrush);
            parts.Enqueue(messagePart);
        }
    }

    private void AddToTextBox(Queue<MessagePart> messageParts)
    {
        if (textBlock.Inlines.Count == 0)
        {
            lines = 0;
        }

        lines++;
        var newInline = new Queue<Run>(messageParts.Count);
        while (messageParts.TryDequeue(out var messagePart))
        {
            var foregroundBrush = messagePart.ForeGround.HasValue
                ? new SolidColorBrush(messagePart.ForeGround.Value)
                : Brushes.Black;
            var backgroundBrush = messagePart.BackGround.HasValue
                ? new SolidColorBrush(messagePart.BackGround.Value)
                : null;
            var run = new Run(messagePart.Text)
            {
                Background = backgroundBrush,
                Foreground = foregroundBrush,
            };

            newInline.Enqueue(run);
        }

        textBlock.Inlines.AddRange(newInline);
        newInline.Clear();

        while (lines > MaxMessages
            && textBlock.Inlines.Count > 0)
        {
            var firstItem = textBlock.Inlines.FirstInline;
            _ = textBlock.Inlines.Remove(textBlock.Inlines.FirstInline);
            if (firstItem is Run oldRun
                && oldRun.Text.EndsWith(Environment.NewLine, StringComparison.Ordinal))
            {
                lines--;
            }
        }
    }

    private record struct MessagePart(string Text, Color? ForeGround, Color? BackGround);
}
