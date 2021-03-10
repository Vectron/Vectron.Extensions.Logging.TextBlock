using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using VectronsLibrary.TextBlockLogger;

namespace Microsoft.Extensions.Logging
{
    internal class TextBlockLoggerFormatterConfigureOptions<TFormatter, TOptions> : ConfigureFromConfigurationOptions<TOptions>
        where TOptions : TextBlockFormatterOptions
        where TFormatter : TextBlockFormatter
    {
        public TextBlockLoggerFormatterConfigureOptions(ILoggerProviderConfiguration<TextBlockLoggerProvider> providerConfiguration) :
            base(providerConfiguration.Configuration.GetSection("FormatterOptions"))
        {
        }
    }
}