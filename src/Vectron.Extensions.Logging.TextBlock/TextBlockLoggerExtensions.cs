using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Vectron.Extensions.Logging.TextBlock.Formatters;
using Vectron.Extensions.Logging.TextBlock.Internal;

namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// Extensions for the <see cref="ILoggingBuilder"/>.
/// </summary>
public static class TextBlockLoggerExtensions
{
    /// <summary>
    /// Add a console log formatter named 'json' to the factory with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddJsonTextBlock(this ILoggingBuilder builder) =>
        builder.AddFormatterWithName(TextBlockFormatterNames.Json);

    /// <summary>
    /// Add and configure a console log formatter named 'json' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="TextBlockLogger"/> options for the built-in json log formatter.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddJsonTextBlock(this ILoggingBuilder builder, Action<JsonTextBlockFormatterOptions> configure)
        => builder.AddTextBlockWithFormatter(TextBlockFormatterNames.Json, configure);

    /// <summary>
    /// Add the default text block log formatter named 'simple' to the factory with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddSimpleTextBlock(this ILoggingBuilder builder) =>
        builder.AddFormatterWithName(TextBlockFormatterNames.Simple);

    /// <summary>
    /// Add and configure a console log formatter named 'simple' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="TextBlockLogger"/> options for the built-in default log formatter.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddSimpleTextBlock(this ILoggingBuilder builder, Action<SimpleTextBlockFormatterOptions> configure)
        => builder.AddTextBlockWithFormatter(TextBlockFormatterNames.Simple, configure);

    /// <summary>
    /// Add and configure a console log formatter named 'systemd' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="TextBlockLogger"/> options for the built-in systemd log formatter.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddSystemdTextBlock(this ILoggingBuilder builder, Action<TextBlockFormatterOptions> configure)
        => builder.AddTextBlockWithFormatter(TextBlockFormatterNames.Systemd, configure);

    /// <summary>
    /// Add a console log formatter named 'systemd' to the factory with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddSystemdTextBlock(this ILoggingBuilder builder)
        => builder.AddFormatterWithName(TextBlockFormatterNames.Systemd);

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

        _ = builder.AddTextBlockFormatter<JsonTextBlockFormatter, JsonTextBlockFormatterOptions>();
        _ = builder.AddTextBlockFormatter<SystemdTextBlockFormatter, TextBlockFormatterOptions>();
        _ = builder.AddTextBlockFormatter<SimpleTextBlockFormatter, SimpleTextBlockFormatterOptions>();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TextBlockLoggerProvider>());
        LoggerProviderOptions.RegisterProviderOptions<TextBlockLoggerOptions, TextBlockLoggerProvider>(builder.Services);
        _ = builder.Services.AddSingleton<ITextBlockProvider, TextBlockProvider>();
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
