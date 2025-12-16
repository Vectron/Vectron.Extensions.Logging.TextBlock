namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// A provider for the <see cref="TextBlock"/>.
/// </summary>
public interface ITextBlockProvider
{
    /// <summary>
    /// Gets the <see cref="TextBlock"/> s to show the log in.
    /// </summary>
    public IEnumerable<ITextBlock> Sinks
    {
        get;
    }

    /// <summary>
    /// Add a <see cref="TextBlock"/> sink.
    /// </summary>
    /// <param name="textBlock">The <see cref="TextBlock"/> to add.</param>
    public void AddTextBlock(System.Windows.Controls.TextBlock textBlock);

    /// <summary>
    /// Remove a <see cref="TextBlock"/> sink.
    /// </summary>
    /// <param name="textBlock">The <see cref="TextBlock"/> to remove.</param>
    public void RemoveTextBlock(System.Windows.Controls.TextBlock textBlock);
}
