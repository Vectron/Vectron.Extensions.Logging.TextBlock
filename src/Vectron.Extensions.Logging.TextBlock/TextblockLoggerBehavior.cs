using Microsoft.Xaml.Behaviors;

namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// A behavior for marking a <see cref="TextBlock"/> for logging output.
/// </summary>
public class TextBlockLoggerBehavior : Behavior<System.Windows.Controls.TextBlock>
{
    /// <summary>
    /// Gets or sets the <see cref="ITextBlockProvider"/> to register to.
    /// </summary>
    internal static ITextBlockProvider? TextBlockProvider
    {
        get;
        set;
    }

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        base.OnAttached();
        if (TextBlockProvider == null)
        {
            return;
        }

        TextBlockProvider.AddTextBlock(AssociatedObject);
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (TextBlockProvider == null)
        {
            return;
        }

        TextBlockProvider.RemoveTextBlock(AssociatedObject);
    }
}
