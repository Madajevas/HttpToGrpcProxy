using Grpc.Core;

using HttpToGrpcProxy;

using System.Threading.Tasks;

namespace ProxyInterceptorTestsClient
{
    public class ResponseContext
    {
        private readonly IClientStreamWriter<Response> clientStreamWriter;

        public Request Request { get; }

        public ResponseContext(Request request, IClientStreamWriter<Response> clientStreamWriter)
        {
            Request = request;
            this.clientStreamWriter = clientStreamWriter;
        }

        public async Task Respond(Response response)
        {
            if (string.IsNullOrWhiteSpace(response.Route))
            {
                response.Route = Request.Route;
            }

            await clientStreamWriter.WriteAsync(response);
        }
    }
}
