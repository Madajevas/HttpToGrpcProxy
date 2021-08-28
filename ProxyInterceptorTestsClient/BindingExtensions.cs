using HttpToGrpcProxy;
using HttpToGrpcProxy.Commons;

using System.Text.Json;

namespace ProxyInterceptorTestsClient
{
    public static class BindingExtensions
    {
        public static T BindJson<T>(this GrpcPromiseContext<Request> grpcPromiseContext)
        {
            return JsonSerializer.Deserialize<T>(grpcPromiseContext.Value.Body);
        }
    }
}
