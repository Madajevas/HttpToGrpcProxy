using Microsoft.Extensions.Logging;

using NUnit.Framework;

namespace HttpToGrpcProxy.Tests.Logging
{
    class ProxyErrorLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Error;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                Assert.Fail($"Error of {logLevel} from proxy: {formatter(state, exception)}");
            }
        }
    }
}
