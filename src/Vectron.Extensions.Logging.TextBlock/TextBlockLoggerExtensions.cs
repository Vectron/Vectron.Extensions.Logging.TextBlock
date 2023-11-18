using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Vectron.Extensions.Logging.TextBlock.Internal;
using Vectron.Extensions.Logging.TextBlock.Themes;

namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// Extensions for the <see cref="ILoggingBuilder"/>.
/// </summary>
public static class TextBlockLoggerExtensions
{
    /// <summary>
    /// Adds a TextBlock logger named 'TextBlock' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="TextBlockLogger"/>.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddTextBlock(this ILoggingBuilder builder, Action<TextBlockLoggerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _ = builder.AddTextBlock();
        _ = builder.Services.Configure(configure);
        return builder;
    }

    /// <summary>
    /// Adds a TextBlock logger named 'TextBlock' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddTextBlock(this ILoggingBuilder builder)
    {
        builder.AddConfiguration();
        _ = builder.AddTextBlockFormatter<ThemedTextBlockFormatter, ThemedTextBlockFormatterOptions>();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TextBlockLoggerProvider>());
        LoggerProviderOptions.RegisterProviderOptions<TextBlockLoggerOptions, TextBlockLoggerProvider>(builder.Services);
        _ = builder.Services.AddSingleton<ITextBlockProvider, TextBlockProvider>();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITheme, NoColorTheme>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITheme, MELTheme>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITheme, MELDarkTheme>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITheme, NLogTheme>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITheme, NLogDarkTheme>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITheme, SerilogTheme>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITheme, SerilogDarkTheme>());
        builder.Services.TryAddSingleton<IThemeProvider, ThemeProvider>();
        return builder;
    }

    /// <summary>
    /// Adds a custom text block logger formatter 'TFormatter' to be configured with options 'TOptions'.
    /// </summary>
    /// <typeparam name="TFormatter">
    /// A <see cref="TextBlockFormatter"/> to use when formatting the text.
    /// </typeparam>
    /// <typeparam name="TOptions">
    /// The <see cref="TextBlockFormatterOptions"/> to pass to the formatter.
    /// </typeparam>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">
    /// A delegate to configure options 'TOptions' for custom formatter 'TFormatter'.
    /// </param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddTextBlockFormatter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TFormatter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>(this ILoggingBuilder builder, Action<TOptions> configure)
        where TOptions : TextBlockFormatterOptions
        where TFormatter : TextBlockFormatter
    {
        ArgumentNullException.ThrowIfNull(configure);
        _ = builder.AddTextBlockFormatter<TFormatter, TOptions>();
        _ = builder.Services.Configure(configure);
        return builder;
    }

    /// <summary>
    /// Adds a custom text block logger formatter 'TFormatter' to be configured with options 'TOptions'.
    /// </summary>
    /// <typeparam name="TFormatter">
    /// A <see cref="TextBlockFormatter"/> to use when formatting the text.
    /// </typeparam>
    /// <typeparam name="TOptions">
    /// The <see cref="TextBlockFormatterOptions"/> to pass to the formatter.
    /// </typeparam>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddTextBlockFormatter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TFormatter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>(this ILoggingBuilder builder)
        where TOptions : TextBlockFormatterOptions
        where TFormatter : TextBlockFormatter
    {
        builder.AddConfiguration();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<TextBlockFormatter, TFormatter>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TOptions>, TextBlockLoggerFormatterConfigureOptions<TFormatter, TOptions>>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TOptions>, TextBlockLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions>>());
        return builder;
    }

    /// <summary>
    /// Add the TextBlock log formatter named 'themed' to the factory with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> for chaining.</returns>
    public static ILoggingBuilder AddThemedTextBlock(this ILoggingBuilder builder)
        => builder.AddFormatterWithName(TextBlockFormatterNames.Themed);

    /// <summary>
    /// Add and configure a TextBlock log formatter named 'themed' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">
    /// A delegate to configure the <see cref="TextBlockLogger"/> options for the built-in default
    /// log formatter.
    /// </param>
    /// <returns>The <see cref="ILoggingBuilder"/> for chaining.</returns>
    public static ILoggingBuilder AddThemedTextBlock(this ILoggingBuilder builder, Action<ThemedTextBlockFormatterOptions> configure)
        => builder.AddTextBlockWithFormatter(TextBlockFormatterNames.Themed, configure);

    /// <summary>
    /// Add a text block logger with a named formatter.
    /// </summary>
    /// <typeparam name="TOptions">
    /// The <see cref="TextBlockFormatterOptions"/> to pass to the formatter.
    /// </typeparam>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="name">The name of the formatter to use.</param>
    /// <param name="configure">
    /// A delegate to configure options 'TOptions' for custom formatter 'TFormatter'.
    /// </param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    internal static ILoggingBuilder AddTextBlockWithFormatter<TOptions>(this ILoggingBuilder builder, string name, Action<TOptions> configure)
        where TOptions : TextBlockFormatterOptions
    {
        ArgumentNullException.ThrowIfNull(configure);
        _ = builder.AddFormatterWithName(name);
        _ = builder.Services.Configure(configure);

        return builder;
    }

    private static ILoggingBuilder AddFormatterWithName(this ILoggingBuilder builder, string name)
        => builder.AddTextBlock((options) => options.FormatterName = name);
}
