using Grpc.Core;
using Grpc.Net.Client;

using HttpToGrpcProxy;
using HttpToGrpcProxy.Commons;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyInterceptorTestsClient
{
    public class Client : IDisposable
    {
        private AsyncDuplexStreamingCall<Response, Request> handle;
        private CancellationTokenSource stopTokenSource;
        private GrpcPromisesFactory<Response, Request> responseFactory;

        public Client(Uri proxyAddress)
        {
            var channel = GrpcChannel.ForAddress(proxyAddress);
            var grpcClient = new Proxy.ProxyClient(channel);
            handle = grpcClient.OnMessage();

            stopTokenSource = new CancellationTokenSource();

            (responseFactory, _) = GrpcPromisesFactory<Response, Request>.Initialize(handle.RequestStream, handle.ResponseStream, stopTokenSource.Token);
        }

        public async Task<RequestContext> InterceptRequest(string route)
        {
            var grpcPromiseContext = await responseFactory[route].Task;

            return new RequestContext(grpcPromiseContext, responseFactory);
        }

        public Task<GrpcPromiseContext<Request>> InterceptRequest(string route, TimeSpan timeout)
        {
            var cancellationSource = new CancellationTokenSource(timeout);
            cancellationSource.Token.Register(() => responseFactory[route].SetCanceled());

            return responseFactory[route].Task;
        }

        public void Dispose()
        {
            stopTokenSource.Cancel();
            handle.Dispose();
        }
    }
}
