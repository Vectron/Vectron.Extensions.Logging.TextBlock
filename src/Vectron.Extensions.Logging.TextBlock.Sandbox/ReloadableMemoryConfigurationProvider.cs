using System.Collections;
using Microsoft.Extensions.Configuration;

namespace Vectron.Extensions.Logging.TextBlock.Sandbox;

/// <summary>
/// In-memory implementation of <see cref="IConfigurationProvider"/>.
/// </summary>
internal sealed class ReloadableMemoryConfigurationProvider : ConfigurationProvider, IEnumerable<KeyValuePair<string, string?>>
{
    private readonly ReloadableMemoryConfigurationSource source;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReloadableMemoryConfigurationProvider"/> class.
    /// </summary>
    /// <param name="source">The source settings.</param>
    public ReloadableMemoryConfigurationProvider(ReloadableMemoryConfigurationSource source)
    {
        ArgumentNullException.ThrowIfNull(source);
        this.source = source;

        if (this.source.InitialData != null)
        {
            foreach (var pair in this.source.InitialData)
            {
                Data.Add(pair.Key, pair.Value);
            }
        }
    }

    /// <summary>
    /// Add a new key and value pair.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <param name="value">The configuration value.</param>
    public void Add(string key, string? value)
    {
        Data.Add(key, value);
        OnReload();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
        => Data.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override void Set(string key, string? value)
    {
        base.Set(key, value);
        OnReload();
    }
}
