using NUnit.Framework;

using RestSharp;

namespace HttpToGrpcProxy.Tests
{
    public class PasingHeadersTests : IntegrationTestsBase
    {
        [Test, Order(1)]
        public async Task RequestHeadersAreForwarded()
        {
            var request = new RestRequest("/anything/anywhere");
            request.AddHeader("MAKE-YOUR-OWN-KIND-OF-MUSIC", "SING-YOUR-OWN-SPECIAL-SONGS");
            var resultPromise = HttpClient.GetAsync<string>(request);
            var requestContext = await Proxy.InterceptRequest("anything/anywhere");

            Assert.That(requestContext.Value.Headers.Values.Count, Is.GreaterThan(1));
            Assert.That(requestContext.Value.Headers.Values["MAKE-YOUR-OWN-KIND-OF-MUSIC"], Is.EqualTo("SING-YOUR-OWN-SPECIAL-SONGS"));
        }
    }
}
