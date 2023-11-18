using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Vectron.Extensions.Logging.TextBlock.Internal;

namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// A class for configuring <see cref="TextBlockFormatterOptions"/> instances.
/// </summary>
/// <typeparam name="TFormatter">The type of formatter.</typeparam>
/// <typeparam name="TOptions">The type of options to bind.</typeparam>
/// <remarks>
/// Initializes a new instance of the
/// <see cref="TextBlockLoggerFormatterConfigureOptions{TFormatter, TOptions}"/> class.
/// </remarks>
/// <param name="providerConfiguration"><see cref="ILoggerProviderConfiguration{T}"/>.</param>
internal sealed class TextBlockLoggerFormatterConfigureOptions<TFormatter, TOptions>(ILoggerProviderConfiguration<TextBlockLoggerProvider> providerConfiguration) : ConfigureFromConfigurationOptions<TOptions>(providerConfiguration.Configuration.GetSection("FormatterOptions"))
    where TOptions : TextBlockFormatterOptions
    where TFormatter : TextBlockFormatter
{
}
