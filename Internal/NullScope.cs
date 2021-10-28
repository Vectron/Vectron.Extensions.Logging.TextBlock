using System;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// An empty scope without any logic.
/// </summary>
internal class NullScope : IDisposable
{
    private NullScope()
    {
    }

    /// <summary>
    /// Gets the default <see cref="NullScope"/> instance.
    /// </summary>
    public static NullScope Instance { get; } = new NullScope();

    /// <inheritdoc />
    public void Dispose()
    {
    }
}