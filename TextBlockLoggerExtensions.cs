using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Windows.Controls;
using VectronsLibrary.TextBlockLogger;

namespace Microsoft.Extensions.Logging
{
    public static class TextBlockLoggerExtensions
    {
        /// <summary>
        /// Adds a TextBlock logger named 'TextBlock' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        public static ILoggingBuilder AddTextBlock(this ILoggingBuilder builder, TextBlock textBlock)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TextBlockLoggerProvider>(x =>
            {
                var options = x.GetService<IOptionsMonitor<TextBlockLoggerOptions>>();
                return new TextBlockLoggerProvider(options, textBlock);
            }));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TextBlockLoggerOptions>, TextBlockLoggerOptionsSetup>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TextBlockLoggerOptions>, LoggerProviderOptionsChangeTokenSource<TextBlockLoggerOptions, TextBlockLoggerProvider>>());
            return builder;
        }

        /// <summary>
        /// Adds a TextBlock logger named 'TextBlock' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure"></param>
        public static ILoggingBuilder AddTextBlock(this ILoggingBuilder builder, TextBlock textBlock, Action<TextBlockLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddTextBlock(textBlock);
            builder.Services.Configure(configure);

            return builder;
        }

        /// <summary>
        /// Adds a TextBlock logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        /// <param name="factory">The <see cref="ILoggerFactory"/> to use.</param>
        public static ILoggerFactory AddTextBlock(this ILoggerFactory factory, TextBlock textBlock)
        {
            return factory.AddTextBlock(textBlock, includeScopes: false);
        }

        /// <summary>
        /// Adds a TextBlock logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        /// <param name="factory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static ILoggerFactory AddTextBlock(this ILoggerFactory factory, TextBlock textBlock, bool includeScopes)
        {
            factory.AddTextBlock(textBlock, (n, l) => l >= LogLevel.Information, includeScopes);
            return factory;
        }

        /// <summary>
        /// Adds a TextBlock logger that is enabled for <see cref="LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="factory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged</param>
        public static ILoggerFactory AddTextBlock(this ILoggerFactory factory, TextBlock textBlock, LogLevel minLevel)
        {
            factory.AddTextBlock(textBlock, minLevel, includeScopes: false);
            return factory;
        }

        /// <summary>
        /// Adds a TextBlock logger that is enabled for <see cref="LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="factory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged</param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static ILoggerFactory AddTextBlock(this ILoggerFactory factory, TextBlock textBlock, LogLevel minLevel, bool includeScopes)
        {
            factory.AddTextBlock(textBlock, (category, logLevel) => logLevel >= minLevel, includeScopes);
            return factory;
        }

        /// <summary>
        /// Adds a TextBlock logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <param name="filter">The category filter to apply to logs.</param>
        public static ILoggerFactory AddTextBlock(this ILoggerFactory factory, TextBlock textBlock, Func<string, LogLevel, bool> filter)
        {
            factory.AddTextBlock(textBlock, filter, includeScopes: false);
            return factory;
        }

        /// <summary>
        /// Adds a TextBlock logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <param name="filter">The category filter to apply to logs.</param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static ILoggerFactory AddTextBlock(this ILoggerFactory factory, TextBlock textBlock, Func<string, LogLevel, bool> filter, bool includeScopes)
        {
            factory.AddProvider(new TextBlockLoggerProvider(filter, includeScopes, textBlock));
            return factory;
        }

        /// <summary>
        /// </summary>
        /// <param name="factory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <param name="settings">The settings to apply to created <see cref="TextBlockLogger"/>'s.</param>
        /// <returns></returns>
        public static ILoggerFactory AddTextBlock(this ILoggerFactory factory, TextBlock textBlock, ITextBlockLoggerSettings settings)
        {
            factory.AddProvider(new TextBlockLoggerProvider(settings, textBlock));
            return factory;
        }

        /// <summary>
        /// </summary>
        /// <param name="factory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> to use for <see cref="ITextBlockLoggerSettings"/>.</param>
        /// <returns></returns>
        public static ILoggerFactory AddTextBlock(this ILoggerFactory factory, TextBlock textBlock, IConfiguration configuration)
        {
            var settings = new ConfigurationTextBlockLoggerSettings(configuration);
            return factory.AddTextBlock(textBlock, settings);
        }
    }
}