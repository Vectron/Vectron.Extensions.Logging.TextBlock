using Microsoft.Extensions.Options;
using System;

namespace VectronsLibrary.TextBlockLogger
{
    internal class FormatterOptionsMonitor<TOptions> :
               IOptionsMonitor<TOptions>
           where TOptions : TextBlockFormatterOptions
    {
        private TOptions options;

        public FormatterOptionsMonitor(TOptions options)
        {
            this.options = options;
        }

        public TOptions CurrentValue => options;

        public TOptions Get(string name) => options;

        public IDisposable OnChange(Action<TOptions, string> listener)
        {
            return Disposable.Empty;
        }
    }
}