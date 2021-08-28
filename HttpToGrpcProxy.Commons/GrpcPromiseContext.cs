using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpToGrpcProxy.Commons
{
    public class GrpcPromiseContext<T> where T : IRoute
    {
        private readonly T value;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<GrpcPromiseContext<T>>> promises;

        /// <summary>
        /// Will self destruct after first use
        /// </summary>
        public T Value
        {
            get
            {
                promises.Remove(value.GetRoute(), out var _);

                return value;
            }
        }

        public GrpcPromiseContext(T response, ConcurrentDictionary<string, TaskCompletionSource<GrpcPromiseContext<T>>> promises)
        {
            this.value = response;
            this.promises = promises;
        }
    }
}
