using HttpToGrpcProxy.Commons;


namespace HttpToGrpcProxy
{
    public partial class Request : IRoute
    {
        public string GetRoute() => Route;
    }

    public partial class Response : IRoute
    {
        public string GetRoute() => Route;
    }
}
