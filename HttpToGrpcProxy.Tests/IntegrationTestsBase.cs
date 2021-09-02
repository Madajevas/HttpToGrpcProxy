using HttpToGrpcProxy.Tests.Deserializers;

using Microsoft.AspNetCore.Builder;

using NUnit.Framework;

using ProxyInterceptorTestsClient;

using RestSharp;

namespace HttpToGrpcProxy.Tests
{
    public class IntegrationTestsBase : TestBase
    {
        private WebApplication app;
        protected RestClient HttpClient;

        public IntegrationTestsBase() : base(new Uri("http://localhost:6000")) { }

        [OneTimeSetUp]
        public void InitializeHttpClientAndProxyServer()
        {
            app = Program.CreateApplication(Array.Empty<string>());
            app.RunAsync().ContinueWith(t => System.Diagnostics.Debugger.Break());

            HttpClient = new RestClient("http://localhost:5000");
            HttpClient.AddHandler("text/plain", new TextPlainDeserializer());
        }

        [OneTimeTearDown]
        public Task StopProxyServer()
        {
            return app.StopAsync();
        }
    }
}
