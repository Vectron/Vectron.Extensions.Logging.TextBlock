using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;

namespace VectronsLibrary.TextBlockLogger
{
    public abstract class TextBlockFormatter
    {
        protected TextBlockFormatter(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Gets the name associated with the textblock log formatter.
        /// </summary>
        public string Name
        {
            get;
        }

        public abstract (string logLevelString, LevelColors logLevelColors) LogLevelData<TState>(in LogEntry<TState> logEntry);

        /// <summary>
        /// Writes the log message to the specified TextWriter.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="scopeProvider">The provider of scope data.</param>
        /// <param name="textWriter">The string writer embedding ansi code for colors.</param>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter);
    }
}