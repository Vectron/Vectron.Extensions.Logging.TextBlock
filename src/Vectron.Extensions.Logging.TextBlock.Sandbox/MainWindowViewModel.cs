using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Theming;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Vectron.Extensions.Logging.TextBlock.Sandbox;

/// <summary>
/// The main windows view model.
/// </summary>
internal sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly IConfiguration configuration;
    private readonly ILogger<MainWindowViewModel> logger;
    private readonly ThemeManager themeManager;

    [ObservableProperty]
    private IEnumerable<Theme> applicationThemes;

    [ObservableProperty]
    private Theme? selectedApplicationTheme;

    [ObservableProperty]
    private string? selectedLoggingTheme;

    [ObservableProperty]
    private IEnumerable<string> themes;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="logger">A <see cref="ILogger"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <param name="loggingThemes">The registered <see cref="ITheme"/>.</param>
    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IConfiguration configuration, IEnumerable<ITheme> loggingThemes)
    {
        this.logger = logger;
        this.configuration = configuration;

        themeManager = ThemeManager.Current;
        var sortedThemes = themeManager.Themes.OrderBy(x => x.ColorScheme, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.BaseColorScheme, StringComparer.OrdinalIgnoreCase);
        applicationThemes = new ReadOnlyObservableCollection<Theme>(new ObservableCollection<Theme>(sortedThemes));
        selectedApplicationTheme = themeManager.DetectTheme() ?? applicationThemes.First();

        var themesCollection = new ObservableCollection<string>(loggingThemes.Select(x => x.Name));
        themes = new ReadOnlyObservableCollection<string>(themesCollection);
        selectedLoggingTheme = configuration["Logging:TextBlock:FormatterOptions:Theme"] ?? "None";
    }

    [RelayCommand]
    private void ChangeApplicationTheme(Theme theme) => _ = themeManager.ChangeTheme(Application.Current, theme);

    [RelayCommand]
    private void ChangeLoggingTheme(string theme) => configuration["Logging:TextBlock:FormatterOptions:Theme"] = theme;

    [RelayCommand]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Testing only.")]
    private void GenerateLogMessages()
    {
        logger.LogTrace("This is a trace message.");
        logger.LogDebug("This is a debug message.");
        logger.LogInformation("This is a information message.");
        logger.LogWarning("This is a warning message.");
        logger.LogError("This is a error message.");
        logger.LogCritical("This is a critical message.");
    }
}
