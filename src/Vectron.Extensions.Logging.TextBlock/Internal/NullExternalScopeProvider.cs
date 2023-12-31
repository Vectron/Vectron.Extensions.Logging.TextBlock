using Microsoft.Extensions.Logging;

namespace Vectron.Extensions.Logging.TextBlock.Internal;

/// <summary>
/// Scope provider that does nothing.
/// </summary>
internal sealed class NullExternalScopeProvider : IExternalScopeProvider
{
    static NullExternalScopeProvider()
        => Instance = new NullExternalScopeProvider();

    private NullExternalScopeProvider()
    {
    }

    /// <summary>
    /// Gets a cached instance of <see cref="NullExternalScopeProvider"/>.
    /// </summary>
    public static IExternalScopeProvider Instance
    {
        get;
    }

    /// <inheritdoc/>
    void IExternalScopeProvider.ForEachScope<TState>(Action<object?, TState> callback, TState state)
    {
    }

    /// <inheritdoc/>
    IDisposable IExternalScopeProvider.Push(object? state)
        => NullScope.Instance;
}
