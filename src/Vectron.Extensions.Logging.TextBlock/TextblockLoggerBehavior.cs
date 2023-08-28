using System.Windows.Controls;
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

        var closeMenuItem = new MenuItem()
        {
            Header = "Clear",
        };

        closeMenuItem.Click += CloseMenuItem_Click;
        AssociatedObject.ContextMenu ??= new ContextMenu();
        _ = AssociatedObject.ContextMenu.Items.Add(closeMenuItem);
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

    private void CloseMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        => AssociatedObject.Inlines.Clear();
}
