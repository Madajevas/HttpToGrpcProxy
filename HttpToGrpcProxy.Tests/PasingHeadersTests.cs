using NUnit.Framework;

using ProxyInterceptorTestsClient;

using RestSharp;

namespace HttpToGrpcProxy.Tests
{
    public class PasingHeadersTests : IntegrationTestsBase
    {
        private Task<IRestResponse> resultPromise;
        private RequestContext requestContext;

        [OneTimeTearDown]
        public void Dispose()
        {
            requestContext?.Dispose();
        }

        [Test, Order(1)]
        public async Task RequestHeadersAreForwarded()
        {
            var request = new RestRequest("/headers", Method.GET);
            request.AddHeader("YOU-GOTTA-MAKE-YOUR-OWN-KIND-OF-MUSIC", "SING-YOUR-OWN-SPECIAL-SONGS");
            resultPromise = HttpClient.ExecuteAsync(request);

            requestContext = await Proxy.InterceptRequest("/headers");

            Assert.That(requestContext.Headers.Count, Is.GreaterThan(1));
            Assert.That(requestContext.Headers["YOU-GOTTA-MAKE-YOUR-OWN-KIND-OF-MUSIC"], Is.EqualTo("SING-YOUR-OWN-SPECIAL-SONGS"));
        }

        [Test, Order(2)]
        public async Task ResonseHeadersAreForwared()
        {
            var response = new Response {
                Route = "/headers",
                HeadersDictionary = new Dictionary<string, string> { ["MAKE-YOUR-OWN-KIND-OF-MUSIC"] = "EVEN-IF-NOBODY-ELSE-SINGS-ALONG" }
            };

            await Proxy.Respond(response);

            var httpResponse = await resultPromise;

            var expectedHeader = httpResponse.Headers.Where(h => h.Name == "MAKE-YOUR-OWN-KIND-OF-MUSIC").FirstOrDefault();
            Assert.That(expectedHeader.Value, Is.EqualTo("EVEN-IF-NOBODY-ELSE-SINGS-ALONG"));
        }
    }
}
