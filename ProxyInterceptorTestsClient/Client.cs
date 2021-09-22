using Grpc.Core;
using Grpc.Net.Client;

using HttpToGrpcProxy;
using HttpToGrpcProxy.Commons;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyInterceptorTestsClient
{
    public interface IClient : IAsyncDisposable
    {
        Task<IRequestContext> InterceptRequest(string route, CancellationToken cancellationToken = default);
        Task<IRequestContext> InterceptRequest(string route, TimeSpan timeout);
    }

    public class Client : IClient
    {
        private AsyncDuplexStreamingCall<Response, Request> handle;
        private CancellationTokenSource stopTokenSource;
        private GrpcPromisesFactory<Response, Request> responseFactory;
        private GrpcChannel channel;

        public Client(Uri proxyAddress)
        {
            channel = GrpcChannel.ForAddress(proxyAddress);
            var grpcClient = new Proxy.ProxyClient(channel);
            handle = grpcClient.OnMessage();

            stopTokenSource = new CancellationTokenSource();

            (responseFactory, _) = GrpcPromisesFactory<Response, Request>.Initialize(handle.RequestStream, handle.ResponseStream, stopTokenSource.Token);
        }

        public async Task<IRequestContext> InterceptRequest(string route, CancellationToken cancellationToken = default)
        {
            cancellationToken.Register(() => responseFactory[route].SetCanceled());

            var grpcPromiseContext = await responseFactory[route].Task;

            return new RequestContext(grpcPromiseContext, responseFactory);
        }

        public Task<IRequestContext> InterceptRequest(string route, TimeSpan timeout)
        {
            var cancellationSource = new CancellationTokenSource(timeout);

            return InterceptRequest(route, cancellationSource.Token);
        }

        public async ValueTask DisposeAsync()
        {
            stopTokenSource.Cancel();
            handle.Dispose();

            await channel.ShutdownAsync();
        }
    }
}
