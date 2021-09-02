using HttpToGrpcProxy;
using HttpToGrpcProxy.Commons;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ProxyInterceptorTestsClient
{
    public class RequestContext : IDisposable
    {
        private readonly GrpcPromiseContext<Request> grpcPromiseContext;

        private Lazy<Dictionary<string, string>> headers;
        public Dictionary<string, string> Headers => headers.Value;

        public string Route => grpcPromiseContext.Value.Route;

        public string ContentType => grpcPromiseContext.Value.ContentType;

        public string Body => grpcPromiseContext.Value.Body;

        public string Method => grpcPromiseContext.Value.Method;

        private RequestContext(GrpcPromiseContext<Request> grpcPromiseContext)
        {
            this.grpcPromiseContext = grpcPromiseContext;

            headers = new Lazy<Dictionary<string, string>>(() => grpcPromiseContext.Value.Headers.Values.ToDictionary(k => k.Key, v => v.Value));
        }

        public void Dispose() => grpcPromiseContext.Dispose();

        public static implicit operator RequestContext(GrpcPromiseContext<Request> grpcPromiseContext)
        {

            return new RequestContext(grpcPromiseContext);
        }
    }
}
