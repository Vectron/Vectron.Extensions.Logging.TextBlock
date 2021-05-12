using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using VectronsLibrary.TextBlockLogger;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Implementation of <see cref="ConfigurationChangeTokenSource{TOptions}"/> for a <see cref="TextBlockFormatterOptions"/>.
    /// </summary>
    /// <typeparam name="TFormatter">The type of formatter.</typeparam>
    /// <typeparam name="TOptions">The type of options to bind.</typeparam>
    internal class TextBlockLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions> : ConfigurationChangeTokenSource<TOptions>
        where TOptions : TextBlockFormatterOptions
        where TFormatter : TextBlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlockLoggerFormatterOptionsChangeTokenSource{TFormatter, TOptions}"/> class.
        /// </summary>
        /// <param name="providerConfiguration"><see cref="ILoggerProviderConfiguration{T}"/>.</param>
        public TextBlockLoggerFormatterOptionsChangeTokenSource(ILoggerProviderConfiguration<TextBlockLoggerProvider> providerConfiguration)
            : base(providerConfiguration.Configuration.GetSection("FormatterOptions"))
        {
        }
    }
}