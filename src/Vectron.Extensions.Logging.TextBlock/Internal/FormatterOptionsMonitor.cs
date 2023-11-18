using Microsoft.Extensions.Options;

namespace Vectron.Extensions.Logging.TextBlock.Internal;

/// <summary>
/// A <see cref="IOptionsMonitor{TOptions}"/> implementation for <see cref="TextBlockFormatterOptions"/>.
/// </summary>
/// <typeparam name="TOptions">The type of the option.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="FormatterOptionsMonitor{TOptions}"/> class.
/// </remarks>
/// <param name="options">The option to monitor.</param>
internal sealed class FormatterOptionsMonitor<TOptions>(TOptions options) : IOptionsMonitor<TOptions>
        where TOptions : TextBlockFormatterOptions
{
    /// <inheritdoc/>
    public TOptions CurrentValue => options;

    /// <inheritdoc/>
    public TOptions Get(string? name) => CurrentValue;

    /// <inheritdoc/>
    public IDisposable OnChange(Action<TOptions, string> listener)
        => Disposable.Empty;
}
