using Microsoft.Extensions.Logging;

namespace HttpToGrpcProxy.Tests.Logging
{
    class ProxyErrorLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new ProxyErrorLogger();
        }

        public void Dispose()
        {

        }
    }
}
