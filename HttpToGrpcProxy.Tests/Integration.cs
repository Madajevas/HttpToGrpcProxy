using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpToGrpcProxy.Tests
{
    public class Integration
    {
        [SetUp]
        public async Task SetUp()
        {
            Program.Main(Array.Empty<string>());
            await Task.Delay(2000);
        }

        [TearDown]
        public async Task TearDown()
        {
            await Program.app.StopAsync();
        }

        [Test]
        public void Test()
        {
            Assert.Pass();
        }
    }
}
