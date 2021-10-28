using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using VectronsLibrary.TextBlockLogger.Internal;

namespace VectronsLibrary.TextBlockLogger;

/// <summary>
/// Extensions for the <see cref="ILoggingBuilder"/>.
/// </summary>
public static class TextBlockLoggerExtensions
{
    /// <summary>
    /// Add the default textblock log formatter named 'simple' to the factory with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddSimpleTextBlock(this ILoggingBuilder builder)
        => builder.AddFormatterWithName(SimpleTextBlockFormatter.DefaultName);

    /// <summary>
    /// Add and configure a textblock log formatter named 'simple' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="TextBlockLogger"/> options for the built-in default log formatter.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddSimpleTextBlock(this ILoggingBuilder builder, Action<SimpleTextBlockFormatterOptions> configure)
        => builder.AddTextBlockWithFormatter(SimpleTextBlockFormatter.DefaultName, configure);

    /// <summary>
    /// Adds a TextBlock logger named 'TextBlock' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="TextBlockLogger"/>.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddTextBlock(this ILoggingBuilder builder, Action<TextBlockLoggerOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

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
        _ = builder.AddTextBlockFormatter<SimpleTextBlockFormatter, SimpleTextBlockFormatterOptions>();
        _ = builder.Services.AddSingleton<ITextblockProvider, TextblockProvider>();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TextBlockLoggerProvider>());
        LoggerProviderOptions.RegisterProviderOptions<TextBlockLoggerOptions, TextBlockLoggerProvider>(builder.Services);

        return builder;
    }

    /// <summary>
    /// Adds a custom textblock logger formatter 'TFormatter' to be configured with options 'TOptions'.
    /// </summary>
    /// <typeparam name="TFormatter">A <see cref="TextBlockFormatter"/> to use when formatting the text.</typeparam>
    /// <typeparam name="TOptions">The <see cref="TextBlockFormatterOptions"/> to pass to the formatter.</typeparam>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure options 'TOptions' for custom formatter 'TFormatter'.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddTextBlockFormatter<TFormatter, TOptions>(this ILoggingBuilder builder, Action<TOptions> configure)
        where TFormatter : TextBlockFormatter
        where TOptions : TextBlockFormatterOptions
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        _ = builder.AddTextBlockFormatter<TFormatter, TOptions>();
        _ = builder.Services.Configure(configure);
        return builder;
    }

    /// <summary>
    /// Adds a custom textblock logger formatter 'TFormatter' to be configured with options 'TOptions'.
    /// </summary>
    /// <typeparam name="TFormatter">A <see cref="TextBlockFormatter"/> to use when formatting the text.</typeparam>
    /// <typeparam name="TOptions">The <see cref="TextBlockFormatterOptions"/> to pass to the formatter.</typeparam>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    public static ILoggingBuilder AddTextBlockFormatter<TFormatter, TOptions>(this ILoggingBuilder builder)
        where TFormatter : TextBlockFormatter
        where TOptions : TextBlockFormatterOptions
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<TextBlockFormatter, TFormatter>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TOptions>, TextBlockLoggerFormatterConfigureOptions<TFormatter, TOptions>>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TOptions>, TextBlockLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions>>());

        return builder;
    }

    /// <summary>
    /// Add a TextblockLogger with a named formatter.
    /// </summary>
    /// <typeparam name="TOptions">The <see cref="TextBlockFormatterOptions"/> to pass to the formatter.</typeparam>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="name">The name of the formatter to use.</param>
    /// <param name="configure">A delegate to configure options 'TOptions' for custom formatter 'TFormatter'.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
    internal static ILoggingBuilder AddTextBlockWithFormatter<TOptions>(this ILoggingBuilder builder, string name, Action<TOptions> configure)
        where TOptions : TextBlockFormatterOptions
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        _ = builder.AddFormatterWithName(name);
        _ = builder.Services.Configure(configure);

        return builder;
    }

    private static ILoggingBuilder AddFormatterWithName(this ILoggingBuilder builder, string name)
        => builder.AddTextBlock((options) => options.FormatterName = name);
}