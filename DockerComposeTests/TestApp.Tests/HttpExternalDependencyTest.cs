using HttpToGrpcProxy;

using NUnit.Framework;

using ProxyInterceptorTestsClient;

using RestSharp;

using System;
using System.Threading.Tasks;

namespace TestApp.Tests
{
    public class HttpExternalDependencyTest : TestBase
    {
        private RestClient restClient;
        private Task<IRestResponse> responsePromise;

        public HttpExternalDependencyTest() : base(new Uri("http://host.docker.internal:6000"))
        {

        }

        [SetUp]
        public void Setup()
        {
            restClient = new RestClient("http://host.docker.internal:8080");
        }

        [Test, Order(1), Timeout(10_000)]
        public async Task WhenCalling_Endpoint_RequestIsCapturedByProxy()
        {
            responsePromise = restClient.ExecuteAsync(new RestRequest("/first"));
            var request = await Proxy.InterceptRequest("/first");

            Assert.That(request.Method, Is.EqualTo("POST"));
            Assert.That(request.Headers["Host"], Is.EqualTo("first.example.com"));
        }

        [Test, Order(2), Timeout(10_000)]
        public async Task WhenRespondingToRequest_ItIsReceivedBackAsHttpResponse()
        {
            var response = new Response { Route = "/first", Body = "response from test", ContentType = "text/plain" };
            await Proxy.Respond(response);

            var httpResponse = await responsePromise;

            Assert.That(httpResponse.Content, Is.EqualTo("response from test"));
        }
    }
}
