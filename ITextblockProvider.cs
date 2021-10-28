using System.Collections.Generic;
using System.Windows.Controls;

namespace VectronsLibrary.TextBlockLogger;

/// <summary>
/// A provider for the <see cref="TextBlock"/>.
/// </summary>
public interface ITextblockProvider
{
    /// <summary>
    /// Gets the <see cref="TextBlock"/>s to show the log in.
    /// </summary>
    IEnumerable<TextBlock> Sinks
    {
        get;
    }

    /// <summary>
    /// Add a <see cref="TextBlock"/> sink.
    /// </summary>
    /// <param name="textblock">The <see cref="TextBlock"/> to add.</param>
    void AddTextBlock(TextBlock textblock);

    /// <summary>
    /// Remove a <see cref="TextBlock"/> sink.
    /// </summary>
    /// <param name="textblock">The <see cref="TextBlock"/> to remove.</param>
    void RemoveTextBlock(TextBlock textblock);
}