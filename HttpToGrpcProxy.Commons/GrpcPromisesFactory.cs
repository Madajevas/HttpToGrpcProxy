using Grpc.Core;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace HttpToGrpcProxy.Commons
{
    public class GrpcPromisesFactory<TIn, TOut> where TOut : IRoute
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

        public static (GrpcPromisesFactory<TIn, TOut>, Task) Initialize<TIn, TOut>(
            IAsyncStreamWriter<TIn> writeStream,
            IAsyncStreamReader<TOut> readStream,
            CancellationToken cancellationToken) where TOut : IRoute
                                                 where TIn : IRoute
        {
            var factory = new GrpcPromisesFactory<TIn, TOut>(writeStream);

            return (factory, factory.InitReader(readStream, cancellationToken));
        }

        private GrpcPromisesFactory(IAsyncStreamWriter<TIn> writeStream)
        {
            this.writeStream = writeStream;
        }

        public async Task<GrpcPromiseContext<TOut>> SendData(TIn value)
        {
            await writeStream.WriteAsync(value);

            return await this[value.GetRoute()].Task;
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
