using Grpc.Core;

using NUnit.Framework;

using ProxyInterceptorTestsClient;

using System;

namespace HttpToGrpcProxy.Tests
{
    public class ProxyUnavailableTests
    {
        private Client client;

        [SetUp]
        public void SetUp()
        {
            client = new Client(new Uri("http://localhost:6666"));
        }

        [Test]
        public void InterceptRequest_WhenServerIsNotAvailable_Throws()
        {
            var aggregateException = Assert.ThrowsAsync<AggregateException>(() => client.InterceptRequest("notavailable"));
            if (aggregateException.InnerException is not RpcException rpcException)
            {
                Assert.Fail("InnerException expected to be of RpcException type");
                return;
            }

            Assert.That(rpcException.StatusCode, Is.EqualTo(StatusCode.Unavailable));
        }
    }
}
