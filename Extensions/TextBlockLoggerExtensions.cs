using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;
using VectronsLibrary.TextBlockLogger;

namespace Microsoft.Extensions.Logging
{
    public static class TextBlockLoggerExtensions
    {
        /// <summary>
        /// Add the default textblock log formatter named 'simple' to the factory with default properties.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        public static ILoggingBuilder AddSimpleTextBlock(this ILoggingBuilder builder) =>
            builder.AddFormatterWithName(SimpleTextBlockFormatter.DefaultName);

        /// <summary>
        /// Add and configure a textblock log formatter named 'simple' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">A delegate to configure the <see cref="TextBlockLogger"/> options for the built-in default log formatter.</param>
        public static ILoggingBuilder AddSimpleTextBlock(this ILoggingBuilder builder, Action<SimpleTextBlockFormatterOptions> configure)
        {
            return builder.AddTextBlockWithFormatter(SimpleTextBlockFormatter.DefaultName, configure);
        }

        /// <summary>
        /// Adds a TextBlock logger named 'TextBlock' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">A delegate to configure the <see cref="TextBlockLogger"/>.</param>
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
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">A delegate to configure options 'TOptions' for custom formatter 'TFormatter'.</param>
        public static ILoggingBuilder AddTextBlockFormatter<TFormatter, TOptions>(this ILoggingBuilder builder, Action<TOptions> configure)
            where TOptions : TextBlockFormatterOptions
            where TFormatter : TextBlockFormatter
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
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">A delegate to configure options 'TOptions' for custom formatter 'TFormatter'.</param>
        public static ILoggingBuilder AddTextBlockFormatter<TFormatter, TOptions>(this ILoggingBuilder builder)
            where TOptions : TextBlockFormatterOptions
            where TFormatter : TextBlockFormatter
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<TextBlockFormatter, TFormatter>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TOptions>, TextBlockLoggerFormatterConfigureOptions<TFormatter, TOptions>>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TOptions>, TextBlockLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions>>());

            return builder;
        }

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

        private static ILoggingBuilder AddFormatterWithName(this ILoggingBuilder builder, string name) =>
            builder.AddTextBlock((TextBlockLoggerOptions options) => options.FormatterName = name);
    }
}