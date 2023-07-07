namespace Vectron.Extensions.Logging.TextBlock.Internal;

/// <summary>
/// An empty <see cref="IDisposable"/> implementation.
/// </summary>
internal sealed class Disposable : IDisposable
{
    /// <summary>
    /// Gets an <see cref="IDisposable"/> that doesn't do anything.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "IDisposableAnalyzers.Correctness",
        "IDISP012:Property should not return created disposable",
        Justification = "It's supposed to return a IDisposable.")]
    public static IDisposable Empty => new Disposable();

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}
