using NUnit.Framework;

using System.Text.Json;
using RestSharp;
using ProxyInterceptorTestsClient;

namespace HttpToGrpcProxy.Tests
{
    public class BindersTests : IntegrationTestsBase
    {
        class JsonBinding
        {
            public long Id { get;set; }
            public string Name {  get; set; }
        }

        [Test]
        public async Task CanBindRequestBodyToClass()
        {
            var model = new JsonBinding { Id = 1, Name = "name" };
            var json = JsonSerializer.Serialize(model);

            var request = new RestRequest("/anything/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var resultPromise = HttpClient.PostAsync<string>(request);

            using var requestContext = await Proxy.InterceptRequest("anything/json");

            Assert.That(requestContext.ContentType, Is.EqualTo("application/json"));

            var receivedModel = requestContext.BindJson<JsonBinding>();

            Assert.That(receivedModel.Id, Is.EqualTo(1));
            Assert.That(receivedModel.Name, Is.EqualTo("name"));
        }
    }
}
