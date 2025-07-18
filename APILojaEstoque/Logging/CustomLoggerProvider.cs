using Microsoft.Extensions.Logging.Configuration;
using System.Collections.Concurrent;

namespace APILojaEstoque.Logging
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        readonly CustomLogggerProviderConfiguration loggerconfig;

        readonly ConcurrentDictionary<string, CustomerLogger> loggers = new();

        public CustomLoggerProvider(CustomLogggerProviderConfiguration config)
        {
            loggerconfig = config;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd (categoryName, name=> new CustomerLogger (name, loggerconfig));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
