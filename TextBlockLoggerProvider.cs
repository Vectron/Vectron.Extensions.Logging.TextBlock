using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Controls;
using VectronsLibrary.TextBlockLogger.Internal;

namespace VectronsLibrary.TextBlockLogger
{
    [ProviderAlias("TextBlock")]
    public class TextBlockLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private static readonly Func<string, LogLevel, bool> falseFilter = (cat, level) => false;
        private static readonly Func<string, LogLevel, bool> trueFilter = (cat, level) => true;
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly ConcurrentDictionary<string, TextBlockLogger> _loggers = new ConcurrentDictionary<string, TextBlockLogger>();
        private readonly TextBlockLoggerProcessor messageQueue;
        private readonly IDisposable optionsReloadToken;
        private bool _disableColors;
        private bool _includeScopes;
        private ITextBlockLoggerSettings _settings;
        private IExternalScopeProvider scopeProvider;

        public TextBlockLoggerProvider(IOptionsMonitor<TextBlockLoggerOptions> options, TextBlock textBlock)
        {
            // Filter would be applied on LoggerFactory level
            messageQueue = new TextBlockLoggerProcessor(textBlock);
            _filter = trueFilter;
            optionsReloadToken = options.OnChange(ReloadLoggerOptions);
            ReloadLoggerOptions(options.CurrentValue);
        }

        public TextBlockLoggerProvider(ITextBlockLoggerSettings settings, TextBlock textBlock)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            messageQueue = new TextBlockLoggerProcessor(textBlock);

            if (_settings.ChangeToken != null)
            {
                _settings.ChangeToken.RegisterChangeCallback(OnConfigurationReload, null);
            }
        }

        public TextBlockLoggerProvider(Func<string, LogLevel, bool> filter, bool includeScopes, TextBlock textBlock)
            : this(filter, includeScopes, false, textBlock)
        {
        }

        public TextBlockLoggerProvider(Func<string, LogLevel, bool> filter, bool includeScopes, bool disableColors, TextBlock textBlock)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            messageQueue = new TextBlockLoggerProcessor(textBlock);
            _includeScopes = includeScopes;
            _disableColors = disableColors;
        }

        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, CreateLoggerImplementation);
        }

        public void Dispose()
        {
            optionsReloadToken?.Dispose();
            messageQueue.Dispose();
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;
        }

        private TextBlockLogger CreateLoggerImplementation(string name)
        {
            var includeScopes = _settings?.IncludeScopes ?? _includeScopes;
            var disableColors = _disableColors;

            return new TextBlockLogger(name, GetFilter(name, _settings), includeScopes ? scopeProvider : null, messageQueue)
            {
                DisableColors = disableColors
            };
        }

        private Func<string, LogLevel, bool> GetFilter(string name, ITextBlockLoggerSettings settings)
        {
            if (_filter != null)
            {
                return _filter;
            }

            if (settings != null)
            {
                foreach (var prefix in GetKeyPrefixes(name))
                {
                    if (settings.TryGetSwitch(prefix, out LogLevel level))
                    {
                        return (n, l) => l >= level;
                    }
                }
            }

            return falseFilter;
        }

        private IEnumerable<string> GetKeyPrefixes(string name)
        {
            while (!string.IsNullOrEmpty(name))
            {
                yield return name;
                var lastIndexOfDot = name.LastIndexOf('.');
                if (lastIndexOfDot == -1)
                {
                    yield return "Default";
                    break;
                }
                name = name.Substring(0, lastIndexOfDot);
            }
        }

        private IExternalScopeProvider GetScopeProvider()
        {
            if (_includeScopes && scopeProvider == null)
            {
                scopeProvider = new LoggerExternalScopeProvider();
            }
            return _includeScopes ? scopeProvider : null;
        }

        private void OnConfigurationReload(object state)
        {
            try
            {
                // The settings object needs to change here, because the old one is probably holding on
                // to an old change token.
                _settings = _settings.Reload();
                _includeScopes = _settings?.IncludeScopes ?? false;
                messageQueue.MaxMessages = _settings.MaxMessages;

                var scopeProvider = GetScopeProvider();
                foreach (var logger in _loggers.Values)
                {
                    logger.Filter = GetFilter(logger.Name, _settings);
                    logger.ScopeProvider = scopeProvider;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading configuration changes.{Environment.NewLine}{ex}");
            }
            finally
            {
                // The token will change each time it reloads, so we need to register again.
                if (_settings?.ChangeToken != null)
                {
                    _settings.ChangeToken.RegisterChangeCallback(OnConfigurationReload, null);
                }
            }
        }

        private void ReloadLoggerOptions(TextBlockLoggerOptions options)
        {
            _includeScopes = options.IncludeScopes;
            _disableColors = options.DisableColors;
            messageQueue.MaxMessages = options.MaxMessages;
            var scopeProvider = GetScopeProvider();
            foreach (var logger in _loggers.Values)
            {
                logger.ScopeProvider = scopeProvider;
                logger.DisableColors = options.DisableColors;
            }
        }
    }
}