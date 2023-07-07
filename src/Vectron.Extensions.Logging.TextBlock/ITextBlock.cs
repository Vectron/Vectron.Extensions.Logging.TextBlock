using System.Windows.Controls;

namespace VectronsLibrary.TextBlockLogger;

/// <summary>
/// Represents A <see cref="TextBlock"/> we can write string to.
/// </summary>
public interface ITextBlock
{
    /// <summary>
    /// Gets or sets the maximum number of messages.
    /// </summary>
    int MaxMessages
    {
        get;
        set;
    }

    /// <summary>
    /// Write a message to the TextBlock.
    /// </summary>
    /// <param name="message">The message to write.</param>
    void Write(string message);
}