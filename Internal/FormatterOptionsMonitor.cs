using System;
using Microsoft.Extensions.Options;

namespace VectronsLibrary.TextBlockLogger
{
    /// <summary>
    /// A <see cref="IOptionsMonitor{TOptions}"/> implementation for <see cref="TextBlockFormatterOptions"/>.
    /// </summary>
    /// <typeparam name="TOptions">The type of the option.</typeparam>
    internal class FormatterOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
           where TOptions : TextBlockFormatterOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatterOptionsMonitor{TOptions}"/> class.
        /// </summary>
        /// <param name="options">The option to monitor.</param>
        public FormatterOptionsMonitor(TOptions options)
            => CurrentValue = options;

        /// <inheritdoc/>
        public TOptions CurrentValue
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public TOptions Get(string name)
            => CurrentValue;

        /// <inheritdoc/>
        public IDisposable OnChange(Action<TOptions, string> listener)
            => Disposable.Empty;
    }
}