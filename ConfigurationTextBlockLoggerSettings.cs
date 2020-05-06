using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;

namespace VectronsLibrary.TextBlockLogger
{
    internal class ConfigurationTextBlockLoggerSettings : ITextBlockLoggerSettings
    {
        private readonly IConfiguration _configuration;

        public ConfigurationTextBlockLoggerSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            ChangeToken = configuration.GetReloadToken();
        }

        public IChangeToken ChangeToken
        {
            get; private set;
        }

        public bool IncludeScopes
        {
            get
            {
                var value = _configuration["IncludeScopes"];
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                else if (bool.TryParse(value, out bool includeScopes))
                {
                    return includeScopes;
                }
                else
                {
                    var message = $"Configuration value '{value}' for setting '{nameof(IncludeScopes)}' is not supported.";
                    throw new InvalidOperationException(message);
                }
            }
        }

        public int MaxMessages
        {
            get
            {
                var value = _configuration["MaxMessages"];
                if (string.IsNullOrEmpty(value))
                {
                    return 100;
                }
                else if (int.TryParse(value, out var maxMessages))
                {
                    return maxMessages;
                }
                else
                {
                    var message = $"Configuration value '{value}' for setting '{nameof(MaxMessages)}' is not supported.";
                    throw new InvalidOperationException(message);
                }
            }
        }

        public ITextBlockLoggerSettings Reload()
        {
            ChangeToken = null;
            return new ConfigurationTextBlockLoggerSettings(_configuration);
        }

        public bool TryGetSwitch(string name, out LogLevel level)
        {
            var switches = _configuration.GetSection("LogLevel");
            if (switches == null)
            {
                level = LogLevel.None;
                return false;
            }

            var value = switches[name];
            if (string.IsNullOrEmpty(value))
            {
                level = LogLevel.None;
                return false;
            }
            else if (Enum.TryParse<LogLevel>(value, true, out level))
            {
                return true;
            }
            else
            {
                var message = $"Configuration value '{value}' for category '{name}' is not supported.";
                throw new InvalidOperationException(message);
            }
        }
    }
}