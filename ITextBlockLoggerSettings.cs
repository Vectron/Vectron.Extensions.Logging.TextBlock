using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace VectronsLibrary.TextBlockLogger
{
    public interface ITextBlockLoggerSettings
    {
        IChangeToken ChangeToken
        {
            get;
        }

        bool IncludeScopes
        {
            get;
        }

        int MaxMessages
        {
            get;
        }

        ITextBlockLoggerSettings Reload();

        bool TryGetSwitch(string name, out LogLevel level);
    }
}