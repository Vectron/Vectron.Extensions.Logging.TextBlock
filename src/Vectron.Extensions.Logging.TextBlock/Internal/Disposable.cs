using System;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// An empty <see cref="IDisposable"/> implementation.
/// </summary>
internal sealed class Disposable : IDisposable
{
    /// <summary>
    /// Gets an <see cref="IDisposable"/> that doesn't do anything.
    /// </summary>
    public static IDisposable Empty => new Disposable();

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}