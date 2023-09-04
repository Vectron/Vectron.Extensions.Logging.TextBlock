using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vectron.Extensions.Hosting.Wpf;
using Vectron.Extensions.Logging.TextBlock;
using Vectron.Extensions.Logging.TextBlock.Sandbox;

var builder = Host.CreateApplicationBuilder(args);

var settings = new List<KeyValuePair<string, string?>>
{
    new KeyValuePair<string, string?>("Logging:TextBlock:FormatterName", "Themed"),
    new KeyValuePair<string, string?>("Logging:TextBlock:LogLevel:Default", "Trace"),
    new KeyValuePair<string, string?>("Logging:TextBlock:MaxMessages", "1000"),
    new KeyValuePair<string, string?>("Logging:TextBlock:FormatterOptions:ColorWholeLine", "false"),
    new KeyValuePair<string, string?>("Logging:TextBlock:FormatterOptions:Theme", "MEL"),
    new KeyValuePair<string, string?>("Logging:TextBlock:FormatterOptions:IncludeScopes", "true"),
    new KeyValuePair<string, string?>("Logging:TextBlock:FormatterOptions:TimestampFormat", "HH:mm:ss"),
    new KeyValuePair<string, string?>("Logging:TextBlock:FormatterOptions:UseUtcTimestamp", "false"),
};

var configBuilder = builder.Configuration as IConfigurationBuilder;
configBuilder.Add(new ReloadableMemoryConfigurationSource { InitialData = settings });

builder.Logging
    .ClearProviders()
    .AddThemedTextBlock();

builder.Services
    .AddWpf<App, MainWindow, MainWindowViewModel>()
    .AddResourceDictionary("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml")
    .AddResourceDictionary("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml")
    .AddResourceDictionary("pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Steel.xaml");

_ = builder.Services;

using var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);
