using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace VectronsLibrary.TextBlockLogger
{
    public class TextBlockLoggerSettings : ITextBlockLoggerSettings
    {
        public IChangeToken ChangeToken
        {
            get; set;
        }

        public bool DisableColors
        {
            get; set;
        }

        public bool IncludeScopes
        {
            get; set;
        }

        public int MaxMessages
        {
            get;
            set;
        }

        public IDictionary<string, LogLevel> Switches { get; set; } = new Dictionary<string, LogLevel>();

        public ITextBlockLoggerSettings Reload()
        {
            return this;
        }

        public bool TryGetSwitch(string name, out LogLevel level)
        {
            return Switches.TryGetValue(name, out level);
        }
    }
}