using System.Windows;

namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// A behavior for marking a <see cref="TextBlock"/> for logging output.
/// </summary>
public static class TextBlockLoggerBehavior
{
    /// <summary>
    /// Identifies the Is Logger Target dependency property.
    /// </summary>
    public static readonly DependencyProperty LoggerTargetProperty = DependencyProperty.RegisterAttached(
        "LoggerTarget",
        typeof(bool),
        typeof(TextBlockLoggerBehavior),
        new PropertyMetadata(defaultValue: false, OnIsLoggerTargetChanged));

    private static readonly DependencyProperty LoggerTargetHandlerProperty = DependencyProperty.RegisterAttached(
        "TextBlockLoggerUnregisterAction",
        typeof(Action),
        typeof(TextBlockLoggerBehavior),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the <see cref="ITextBlockProvider"/> to register to.
    /// </summary>
    internal static ITextBlockProvider? TextBlockProvider
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="TextBlock"/> is used for logging output.
    /// </summary>
    /// <param name="dependencyObject">The <see cref="DependencyObject"/> to check on.</param>
    /// <returns><see langword="true"/> if this is a logging target; else <see langword="false"/>.</returns>
    public static bool GetLoggerTarget(DependencyObject dependencyObject)
    {
        if (dependencyObject is null)
        {
            throw new ArgumentNullException(nameof(dependencyObject));
        }

        return (bool)dependencyObject.GetValue(LoggerTargetProperty);
    }

    /// <summary>
    /// Sets a value indicating whether this <see cref="TextBlock"/> is used for logging output.
    /// </summary>
    /// <param name="dependencyObject">The <see cref="DependencyObject"/> to check on.</param>
    /// <param name="value">The value to set.</param>
    public static void SetLoggerTarget(DependencyObject dependencyObject, bool value)
    {
        if (dependencyObject is null)
        {
            throw new ArgumentNullException(nameof(dependencyObject));
        }

        dependencyObject.SetValue(LoggerTargetProperty, value);
    }

    private static void OnIsLoggerTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (TextBlockProvider == null
            || e.NewValue is not bool isLoggerTarget
            || d is not System.Windows.Controls.TextBlock textBlock)
        {
            return;
        }

        if (d.GetValue(LoggerTargetHandlerProperty) is Action destroyAction)
        {
            textBlock.SetValue(LoggerTargetHandlerProperty, value: null);
        }

        if (isLoggerTarget)
        {
            TextBlockProvider.AddTextBlock(textBlock);
            textBlock.SetValue(LoggerTargetHandlerProperty, () => TextBlockProvider.RemoveTextBlock(textBlock));
        }
    }
}
