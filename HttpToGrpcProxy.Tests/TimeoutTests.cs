using NUnit.Framework;

using System;
using System.Threading.Tasks;

namespace HttpToGrpcProxy.Tests
{
    public class TimeoutTests : IntegrationTestsBase
    {
        [Test]
        public void InteruptAsync_WhenRequestIsNotReceivedInTime_TaskCanceledException()
        {
            // no request was sent, so 'timeout/me' route promise will never get resolved
            Assert.ThrowsAsync<TaskCanceledException>(() => Proxy.InterceptRequest("timeout/me", TimeSpan.FromSeconds(1)));
        }
    }
}
