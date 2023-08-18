using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vectron.Extensions.Logging.TextBlock.Themes;

namespace Vectron.Extensions.Logging.TextBlock.Internal;

/// <summary>
/// The default implementation of <see cref="IThemeProvider"/> using the configured theme.
/// </summary>
internal sealed class ThemeProvider : IThemeProvider, IDisposable
{
    private readonly IDisposable? optionsReloadToken;
    private readonly IEnumerable<ITheme> themes;
    private ITheme currentTheme;
    private ThemedTextBlockFormatterOptions formatterOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeProvider"/> class.
    /// </summary>
    /// <param name="options">Options for setting up this formatter.</param>
    /// <param name="themes">The registered themes.</param>
    public ThemeProvider(IOptionsMonitor<ThemedTextBlockFormatterOptions> options, IEnumerable<ITheme> themes)
    {
        this.themes = themes;
        ReloadLoggerOptions(options.CurrentValue);
        optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <inheritdoc/>
    string ITheme.Name => string.Empty;

    // returns true on MacCatalyst
    private static bool IsAndroidOrAppleMobile => OperatingSystem.IsAndroid() || OperatingSystem.IsTvOS() || OperatingSystem.IsIOS();

    /// <inheritdoc/>
    public void Dispose()
        => optionsReloadToken?.Dispose();

    /// <inheritdoc/>
    public string GetCategoryColor(string category) => currentTheme.GetCategoryColor(category);

    /// <inheritdoc/>
    public string GetEventIdColor(EventId eventId) => currentTheme.GetEventIdColor(eventId);

    /// <inheritdoc/>
    public string GetExceptionColor(Exception exception) => currentTheme.GetExceptionColor(exception);

    /// <inheritdoc/>
    public string GetLineColor(LogLevel logLevel) => currentTheme.GetLineColor(logLevel);

    /// <inheritdoc/>
    public string GetLogLevelColor(LogLevel logLevel) => currentTheme.GetLogLevelColor(logLevel);

    /// <inheritdoc/>
    public string GetMessageColor(string message) => currentTheme.GetMessageColor(message);

    /// <inheritdoc/>
    public string GetScopeColor(object? scope) => currentTheme.GetScopeColor(scope);

    /// <inheritdoc/>
    public string GetTimeColor(DateTimeOffset dateTimeOffset) => currentTheme.GetTimeColor(dateTimeOffset);

    [MemberNotNull(nameof(formatterOptions))]
    [MemberNotNull(nameof(currentTheme))]
    private void ReloadLoggerOptions(ThemedTextBlockFormatterOptions options)
    {
        formatterOptions = options;
        var themeName = formatterOptions.Theme;

        if (IsAndroidOrAppleMobile)
        {
            themeName = "None";
        }
        else if (string.IsNullOrEmpty(themeName))
        {
            themeName = "MEL";
        }

        var theme = themes.FirstOrDefault(x => x.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase));
        currentTheme = theme ?? new NoColorTheme();
    }
}
