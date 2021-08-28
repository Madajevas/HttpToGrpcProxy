
using HttpToGrpcProxy.Commons;

namespace HttpToGrpcProxy;

internal partial class Response : IRoute
{
    public string GetRoute() => Route;
}

internal partial class Request : IRoute
{
    public string GetRoute() => Route;
}
