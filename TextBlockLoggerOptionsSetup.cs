using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace VectronsLibrary.TextBlockLogger
{
    internal class TextBlockLoggerOptionsSetup : ConfigureFromConfigurationOptions<TextBlockLoggerOptions>
    {
        public TextBlockLoggerOptionsSetup(ILoggerProviderConfiguration<TextBlockLoggerProvider> providerConfiguration)
            : base(providerConfiguration.Configuration)
        {
        }
    }
}