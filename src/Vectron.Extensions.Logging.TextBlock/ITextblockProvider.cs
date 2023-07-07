using System.Collections.Generic;
using System.Windows.Controls;

namespace VectronsLibrary.TextBlockLogger;

/// <summary>
/// A provider for the <see cref="TextBlock"/>.
/// </summary>
public interface ITextBlockProvider
{
    /// <summary>
    /// Gets the <see cref="TextBlock"/>s to show the log in.
    /// </summary>
    IEnumerable<ITextBlock> Sinks
    {
        get;
    }

    /// <summary>
    /// Add a <see cref="TextBlock"/> sink.
    /// </summary>
    /// <param name="textBlock">The <see cref="TextBlock"/> to add.</param>
    void AddTextBlock(TextBlock textBlock);

    /// <summary>
    /// Remove a <see cref="TextBlock"/> sink.
    /// </summary>
    /// <param name="textBlock">The <see cref="TextBlock"/> to remove.</param>
    void RemoveTextBlock(TextBlock textBlock);
}