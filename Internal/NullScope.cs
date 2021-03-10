using System;

namespace VectronsLibrary.TextBlockLogger
{
    /// <summary>
    /// An empty scope without any logic
    /// </summary>
    internal class NullScope : IDisposable
    {
        private NullScope()
        {
        }

        public static NullScope Instance { get; } = new NullScope();

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}