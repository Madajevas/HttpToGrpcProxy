using NUnit.Framework;

using RestSharp;

namespace HttpToGrpcProxy.Tests
{
    public class IntegrationTests : IntegrationTestsBase
    {
        [Test, Order(1)]
        public async Task TestRoundTrip()
        {
            var request = new RestRequest("/anything/anywhere");
            // do not block thread when making request
            var resultPromise = HttpClient.GetAsync<string>(request);

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
            var resultPromise = HttpClient.GetAsync<string>(request);
            var requestContext = await Proxy.InterceptRequest("anything/anywhere");

            Assert.That(requestContext.Request.Route, Is.EqualTo("anything/anywhere"));

            var response = new Response { Body = "new content", ContentType = "text/plain" };
            await requestContext.Respond(response);

            var httpResponse = await resultPromise;

            Assert.That(httpResponse, Is.EqualTo("new content"));
        }
    }
}
