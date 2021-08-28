using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpToGrpcProxy.Commons
{
    public class GrpcPromiseContext<T> : IDisposable where T : IRoute
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<GrpcPromiseContext<T>>> promises;

        public T Value { get; private init; }

        public GrpcPromiseContext(T value, ConcurrentDictionary<string, TaskCompletionSource<GrpcPromiseContext<T>>> promises)
        {
            this.Value = value;
            this.promises = promises;
        }

        public void Dispose() => promises.Remove(Value.GetRoute(), out var _);
    }
}
