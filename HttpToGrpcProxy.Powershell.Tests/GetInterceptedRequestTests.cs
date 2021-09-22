using NSubstitute;
using NSubstitute.Core;

using NUnit.Framework;

using ProxyInterceptorTestsClient;

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HttpToGrpcProxy.Powershell.Tests
{
    public class GetInterceptedRequestTests
    {
        private IClient client;
        private GetInterceptedRequest cmdlet;

        [SetUp]
        public void SetUp()
        {
            client = Substitute.For<IClient>();

            cmdlet = new GetInterceptedRequest
            {
                ProxyClient = client,
                Route = "/somewhere"
            };
        }

        private void BlockTask(CallInfo callInfo)
        {
            var token = callInfo.Arg<CancellationToken>();

            while (true)
            {
                token.ThrowIfCancellationRequested();
                Task.Delay(100).Wait();
            }
        }

        [Test]
        public async Task InterceptRequest_IfProcessingGetsCancelled_Throws()
        {
            client.When(c => c.InterceptRequest(Arg.Any<string>(), Arg.Any<CancellationToken>()))
                .Do(BlockTask);

            // method is protected
            var stopProcessing = cmdlet.GetType().GetMethod("StopProcessing", BindingFlags.NonPublic | BindingFlags.Instance);

            var enumerator = cmdlet.Invoke().GetEnumerator();
            // call MoveNext not blockingly
            var moveNextTask = Task.Run(() => enumerator.MoveNext());
            // and wait a bit for
            await Task.Delay(100).ConfigureAwait(false);

            // invoke stop processing, its like clicking Ctrl + C
            stopProcessing.Invoke(cmdlet, Array.Empty<object>());

            // awaiting task should throw
            Assert.That(() => moveNextTask, Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public async Task InterceptRequest_IfInterceptingTimesout_Throws()
        {
            client.When(c => c.InterceptRequest(Arg.Any<string>(), Arg.Any<CancellationToken>()))
                .Do(BlockTask);
            cmdlet.Timeout = TimeSpan.FromMilliseconds(1);

            var enumerator = cmdlet.Invoke().GetEnumerator();
            // call MoveNext not blockingly
            var moveNextTask = Task.Run(() => enumerator.MoveNext());
            // and wait for timeout to occur
            await Task.Delay(100).ConfigureAwait(false);

            // awaiting task should throw
            Assert.That(() => moveNextTask, Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void InterceptRequest_WhenInterceptingRequestSucceeds_ReturnsRequestContext()
        {
            var requestContext = Substitute.For<IRequestContext>();
            client.InterceptRequest(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(requestContext));

            var enumerator = cmdlet.Invoke().GetEnumerator();
            enumerator.MoveNext();

            Assert.That(requestContext is IRequestContext);
        }
    }
}
