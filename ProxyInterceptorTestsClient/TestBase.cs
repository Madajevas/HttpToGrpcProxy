using NUnit.Framework;

using System;
using System.Threading.Tasks;

namespace ProxyInterceptorTestsClient
{
    public class TestBase
    {
        private readonly Uri proxyAddress;

        public TestBase(Uri proxyAddress)
        {
            this.proxyAddress = proxyAddress;
        }

        public Client Proxy { get; private set; }

        [OneTimeSetUp]
        public void InitializeClient()
        {
            Proxy = new Client(proxyAddress);
        }

        [OneTimeTearDown]
        public async Task DisconnectClient()
        {
            await Proxy.DisposeAsync();
        }
    }
}
