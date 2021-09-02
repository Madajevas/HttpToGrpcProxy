using HttpToGrpcProxy;
using HttpToGrpcProxy.Commons;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyInterceptorTestsClient
{
    public class RequestContext : IDisposable
    {
        private readonly GrpcPromiseContext<Request> grpcPromiseContext;
        private readonly GrpcPromisesFactory<Response, Request> responseFactory;
        private Lazy<Dictionary<string, string>> headers;
        public Dictionary<string, string> Headers => headers.Value;

        public string Route => grpcPromiseContext.Value.Route;

        public string ContentType => grpcPromiseContext.Value.ContentType;

        public string Body => grpcPromiseContext.Value.Body;

        public string Method => grpcPromiseContext.Value.Method;

        internal RequestContext(GrpcPromiseContext<Request> grpcPromiseContext, GrpcPromisesFactory<Response, Request> responseFactory)
        {
            this.grpcPromiseContext = grpcPromiseContext;
            this.responseFactory = responseFactory;
            headers = new Lazy<Dictionary<string, string>>(() => grpcPromiseContext.Value.Headers.ToDictionary(k => k.Key, v => v.Value));
        }

        /// <summary>
        /// Respond to request in context. <see cref="Response.Route"/> can be omitted on response object
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public Task Respond(Response response)
        {
            if (string.IsNullOrEmpty(response.Route))
            {
                response.Route = Route;
            }

            return responseFactory.SendData(response);
        }

        public void Dispose() => grpcPromiseContext.Dispose();
    }
}
