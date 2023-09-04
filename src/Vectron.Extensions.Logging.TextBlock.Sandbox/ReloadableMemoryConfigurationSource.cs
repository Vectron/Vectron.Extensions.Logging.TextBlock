using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace Vectron.Extensions.Logging.TextBlock.Sandbox;

/// <summary>
/// Represents in-memory data as an <see cref="IConfigurationSource"/>.
/// That triggers change events.
/// </summary>
internal sealed class ReloadableMemoryConfigurationSource : IConfigurationSource
{
    /// <summary>
    /// Gets or sets the initial key value configuration pairs.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string?>>? InitialData
    {
        get; set;
    }

    /// <summary>
    /// Builds the <see cref="MemoryConfigurationProvider"/> for this source.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
    /// <returns>A <see cref="MemoryConfigurationProvider"/>.</returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new ReloadableMemoryConfigurationProvider(this);
}
