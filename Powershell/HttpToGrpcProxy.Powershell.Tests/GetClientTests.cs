using NUnit.Framework;

using ProxyInterceptorTestsClient;

namespace HttpToGrpcProxy.Powershell.Tests
{
    public class GetClientTests
    {
        private GetClient cmdlet;

        [SetUp]
        public void Setup()
        {
            cmdlet = new GetClient { ProxyAddress = new System.Uri("http://test.test") };
        }

        [Test]
        public void GetClient_ReturnsSingleValueOfTypeClient()
        {
            var enumerator = cmdlet.Invoke().GetEnumerator();
            enumerator.MoveNext();
            var client = enumerator.Current;

            Assert.That(client, Is.InstanceOf<Client>());
            Assert.That(enumerator.MoveNext(), Is.False);
        }
    }
}