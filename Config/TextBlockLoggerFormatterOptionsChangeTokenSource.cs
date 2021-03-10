using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using VectronsLibrary.TextBlockLogger;

namespace Microsoft.Extensions.Logging
{
    internal class TextBlockLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions> : ConfigurationChangeTokenSource<TOptions>
        where TOptions : TextBlockFormatterOptions
        where TFormatter : TextBlockFormatter
    {
        public TextBlockLoggerFormatterOptionsChangeTokenSource(ILoggerProviderConfiguration<TextBlockLoggerProvider> providerConfiguration)
            : base(providerConfiguration.Configuration.GetSection("FormatterOptions"))
        {
        }
    }
}