using System;

namespace VectronsLibrary.TextBlockLogger
{
    internal class Disposable : IDisposable
    {
        public static IDisposable Empty => new Disposable();

        public void Dispose()
        {
        }
    }
}