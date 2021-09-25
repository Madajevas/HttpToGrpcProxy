using Grpc.Core;

using NUnit.Framework;

using ProxyInterceptorTestsClient;

using System;

namespace HttpToGrpcProxy.Tests
{
    public class ProxyUnavailableTests
    {
        private Client client;

        [OneTimeSetUp]
        public void SetUp()
        {
            client = new Client(new Uri("http://localhost:6666"));
        }

        [Test, Order(1)]
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

        [Test, Order(2)]
        public void InterceptRequest_OnSubsequentRequests_Throws()
        {
            Assert.ThrowsAsync<AggregateException>(() => client.InterceptRequest("notavailable"));
        }
    }
}
