using NUnit.Framework;

using System.Text.Json;
using RestSharp;
using ProxyInterceptorTestsClient;

namespace HttpToGrpcProxy.Tests
{
    public class BindersTests : IntegrationTestsBase
    {
        class Model
        {
            public long Id { get;set; }
            public string Name {  get; set; }
        }

        [Test]
        public async Task CanBindRequestBodyToClass()
        {
            var model = new Model { Id = 1, Name = "name" };
            var json = JsonSerializer.Serialize(model);

            var request = new RestRequest("/binding/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var _ = HttpClient.PostAsync<string>(request);

            using var requestContext = await Proxy.InterceptRequest("binding/json");

            Assert.That(requestContext.ContentType, Is.EqualTo("application/json"));

            var receivedModel = requestContext.BindJson<Model>();

            Assert.That(receivedModel.Id, Is.EqualTo(1));
            Assert.That(receivedModel.Name, Is.EqualTo("name"));
        }

        [Test]
        public async Task CanBindFormToClass()
        {
            var request = new RestRequest("binding/form");
            request.AddParameter("id", 42);
            request.AddParameter("name", "test");
            var _ = HttpClient.PostAsync<string>(request);

            using var requestContext = await Proxy.InterceptRequest("binding/form");

            Assert.That(requestContext.ContentType, Is.EqualTo("application/x-www-form-urlencoded"));

            var receivedModel = requestContext.BindForm<Model>();

            Assert.That(receivedModel.Id, Is.EqualTo(42));
            Assert.That(receivedModel.Name, Is.EqualTo("test"));
        }

        [Test]
        public async Task CanBindHeadersToClass()
        {
            var request = new RestRequest("binding/headers");
            request.AddHeader("id", "42");
            request.AddHeader("name", "test");
            var _ = HttpClient.PostAsync<string>(request);

            using var requestContext = await Proxy.InterceptRequest("binding/headers");
            var receivedModel = requestContext.BindHeaders<Model>();

            Assert.That(receivedModel.Id, Is.EqualTo(42));
            Assert.That(receivedModel.Name, Is.EqualTo("test"));
        }

        [Test]
        public async Task CanBindQueryToClass()
        {
            var request = new RestRequest("binding/query?id=42&name=test");
            var _ = HttpClient.GetAsync<string>(request);

            using var requestContext = await Proxy.InterceptRequest("binding/query");
            var receivedModel = requestContext.BindQuery<Model>();

            Assert.That(receivedModel.Id, Is.EqualTo(42));
            Assert.That(receivedModel.Name, Is.EqualTo("test"));
        }
    }
}
