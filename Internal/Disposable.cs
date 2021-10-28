using System;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// An empty <see cref="IDisposable"/> implementation.
/// </summary>
internal class Disposable : IDisposable
{
    /// <summary>
    /// Gets an <see cref="IDisposable"/> that doesnt do anything.
    /// </summary>
    public static IDisposable Empty => new Disposable();

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}