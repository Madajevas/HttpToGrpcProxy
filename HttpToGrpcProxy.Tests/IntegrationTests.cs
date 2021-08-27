using HttpToGrpcProxy.Tests.Deserializers;

using Microsoft.AspNetCore.Builder;

using NUnit.Framework;

using ProxyInterceptorTestsClient;

using RestSharp;

namespace HttpToGrpcProxy.Tests
{
    public class IntegrationTests : TestBase
    {
        private WebApplication app;
        private RestClient httpClient;

        public IntegrationTests() : base(new Uri("http://localhost:6000")) { }

        [OneTimeSetUp]
        public void InitializeHttpClientAndProxyServer()
        {
            app = Program.CreateApplication(Array.Empty<string>());
            app.RunAsync();

            httpClient = new RestClient("http://localhost:5000");
            httpClient.AddHandler("text/plain", new TextPlainDeserializer());
        }

        [OneTimeTearDown]
        public Task StopProxyServer()
        {
            return app.StopAsync();
        }

        [Test, Order(1)]
        public async Task TestRoundTrip()
        {
            var request = new RestRequest("/anything/anywhere");
            // do not block thread when making request
            var resultPromise = httpClient.GetAsync<string>(request);

            // TODO: thim slashes
            // TODO: cancellation token to enable timeout
            // block until request is received
            var requestContext = await Proxy.InterceptRequest("anything/anywhere");

            // assert what ever is necessary
            Assert.That(requestContext.Request.Route, Is.EqualTo("anything/anywhere"));

            // respond to request, block
            var response = new Response { Body = "responding from unit test", ContentType = "text/plain" };
            await requestContext.Respond(response);

            // TODO: some sort of timeout logic is necessary here
            // now wait for response to come back
            var httpResponse = await resultPromise;

            // assert whats necessary
            Assert.That(httpResponse, Is.EqualTo("responding from unit test"));
        }

        [Test, Order(2)]
        public async Task SubsequentRequestsToSamePathMustBeResolvedSeparately()
        {
            var request = new RestRequest("/anything/anywhere");
            var resultPromise = httpClient.GetAsync<string>(request);
            var requestContext = await Proxy.InterceptRequest("anything/anywhere");

            Assert.That(requestContext.Request.Route, Is.EqualTo("anything/anywhere"));

            var response = new Response { Body = "new content", ContentType = "text/plain" };
            await requestContext.Respond(response);

            var httpResponse = await resultPromise;

            Assert.That(httpResponse, Is.EqualTo("new content"));
        }
    }
}
