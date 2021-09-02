using System.Text.Json;

namespace ProxyInterceptorTestsClient
{
    public static class BindingExtensions
    {
        public static T BindJson<T>(this RequestContext requestContext)
        {
            return JsonSerializer.Deserialize<T>(requestContext.Body);
        }
    }
}
