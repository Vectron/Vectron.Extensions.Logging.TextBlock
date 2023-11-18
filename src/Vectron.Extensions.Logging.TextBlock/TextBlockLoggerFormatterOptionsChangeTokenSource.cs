using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Vectron.Extensions.Logging.TextBlock.Internal;

namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// Implementation of <see cref="ConfigurationChangeTokenSource{TOptions}"/> for a <see cref="TextBlockFormatterOptions"/>.
/// </summary>
/// <typeparam name="TFormatter">The type of formatter.</typeparam>
/// <typeparam name="TOptions">The type of options to bind.</typeparam>
/// <remarks>
/// Initializes a new instance of the
/// <see cref="TextBlockLoggerFormatterOptionsChangeTokenSource{TFormatter, TOptions}"/> class.
/// </remarks>
/// <param name="providerConfiguration"><see cref="ILoggerProviderConfiguration{T}"/>.</param>
internal sealed class TextBlockLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions>(ILoggerProviderConfiguration<TextBlockLoggerProvider> providerConfiguration) : ConfigurationChangeTokenSource<TOptions>(providerConfiguration.Configuration.GetSection("FormatterOptions"))
    where TOptions : TextBlockFormatterOptions
    where TFormatter : TextBlockFormatter
{
}
