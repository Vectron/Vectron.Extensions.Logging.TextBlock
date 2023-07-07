using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Vectron.Extensions.Logging.TextBlock.Internal;

namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// A class for configuring <see cref="TextBlockFormatterOptions"/> instances.
/// </summary>
/// <typeparam name="TFormatter">The type of formatter.</typeparam>
/// <typeparam name="TOptions">The type of options to bind.</typeparam>
internal sealed class TextBlockLoggerFormatterConfigureOptions<TFormatter, TOptions> : ConfigureFromConfigurationOptions<TOptions>
    where TOptions : TextBlockFormatterOptions
    where TFormatter : TextBlockFormatter
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="TextBlockLoggerFormatterConfigureOptions{TFormatter, TOptions}"/> class.
    /// </summary>
    /// <param name="providerConfiguration"><see cref="ILoggerProviderConfiguration{T}"/>.</param>
    public TextBlockLoggerFormatterConfigureOptions(ILoggerProviderConfiguration<TextBlockLoggerProvider> providerConfiguration)
        : base(providerConfiguration.Configuration.GetSection("FormatterOptions"))
    {
    }
}
