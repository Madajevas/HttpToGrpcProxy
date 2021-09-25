using Grpc.Core;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace HttpToGrpcProxy.Commons
{
    public class GrpcPromisesFactory<TIn, TOut>
        where TOut : IRoute
        where TIn : IRoute
    {
        private IAsyncStreamWriter<TIn> writeStream;
        private ConcurrentDictionary<string, TaskCompletionSource<GrpcPromiseContext<TOut>>> promises = new ConcurrentDictionary<string, TaskCompletionSource<GrpcPromiseContext<TOut>>>();

        public TaskCompletionSource<GrpcPromiseContext<TOut>> this[string route]
        {
            get
            {
                var sanitizedRoute = route.Trim('/');
                if (!promises.ContainsKey(sanitizedRoute))
                {
                    promises[sanitizedRoute] = new TaskCompletionSource<GrpcPromiseContext<TOut>>();
                }

                return promises[sanitizedRoute];
            }
        }

        public static (GrpcPromisesFactory<TIn, TOut>, Task) Initialize(
            IAsyncStreamWriter<TIn> writeStream,
            IAsyncStreamReader<TOut> readStream,
            CancellationToken cancellationToken)
        {
            var factory = new GrpcPromisesFactory<TIn, TOut>(writeStream);

            return (factory, factory.InitReader(readStream, cancellationToken));
        }

        private GrpcPromisesFactory(IAsyncStreamWriter<TIn> writeStream)
        {
            this.writeStream = writeStream;
        }

        public async Task<GrpcPromiseContext<TOut>> SendAndWaitForResonse(TIn value, CancellationToken cancellationToken)
        {
            await writeStream.WriteAsync(value);

            cancellationToken.Register(() => this[value.GetRoute()].SetCanceled());

            return await this[value.GetRoute()].Task;
        }

        public Task SendData(TIn value) => writeStream.WriteAsync(value);

        public void CancelAll(Exception ex)
        {
            foreach (var (route, promise) in promises)
            {
                promise.TrySetException(ex);
                promises.TryRemove(route, out var _);
            }
        }

        private async Task InitReader(IAsyncStreamReader<TOut> readStream, CancellationToken cancellationToken)
        {
            while (await readStream.MoveNext(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                var message = readStream.Current;

                var responseContext = new GrpcPromiseContext<TOut>(message, promises);
                this[message.GetRoute()].SetResult(responseContext);
            }
        }
    }
}
