using Grpc.Core;
using Grpc.Net.Client;

using HttpToGrpcProxy;

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ProxyInterceptorTestsClient
{
    public class Client : IDisposable
    {
        private ConcurrentDictionary<string, TaskCompletionSource<ResponseContext>> promises = new ConcurrentDictionary<string, TaskCompletionSource<ResponseContext>>();

        private Proxy.ProxyClient grpcClient;
        private AsyncDuplexStreamingCall<Response, Request> handle;

        public TaskCompletionSource<ResponseContext> this[string path]
        {
            get
            {
                if (!promises.ContainsKey(path))
                {
                    promises[path] = new TaskCompletionSource<ResponseContext>();
                }

                return promises[path];
            }
        }

        public Client(Uri proxyAddress)
        {
            var channel = GrpcChannel.ForAddress(proxyAddress);
            grpcClient = new Proxy.ProxyClient(channel);

            handle = grpcClient.OnMessage();

            InitReader(handle.ResponseStream);
        }

        public void Dispose() => handle.Dispose();

        public Task<ResponseContext> InterceptRequest(string route)
        {
            var tsc = new TaskCompletionSource<ResponseContext>();
            promises[route] = tsc;

            return tsc.Task;
        }

        private async Task InitReader(IAsyncStreamReader<Request> responseStream/*, ServerCallContext context*/)
        {
            while (await responseStream.MoveNext()/* && !context.CancellationToken.IsCancellationRequested*/)
            {
                var message = responseStream.Current;
                // logger.LogInformation("Request received {Response}", message);

                this[message.Route].SetResult(new ResponseContext(message, handle.RequestStream));
            }
        }
    }
}
