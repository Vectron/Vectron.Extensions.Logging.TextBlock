namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// Determines when to use color when logging messages.
/// </summary>
public enum LoggerColorBehavior
{
    /// <summary>
    /// Use the default color behavior, enabling color except.
    /// </summary>
    Default,

    /// <summary>
    /// Enable color for logging.
    /// </summary>
    Enabled,

    /// <summary>
    /// Disable color for logging.
    /// </summary>
    Disabled,
}
