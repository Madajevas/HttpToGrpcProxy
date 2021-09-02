using Grpc.Core;

using HttpToGrpcProxy.Commons;

namespace HttpToGrpcProxy.Services;

/// <summary>
/// Intenden for single client
/// </summary>
class ProxyService : Proxy.ProxyBase
{
    private readonly ILogger<ProxyService> logger;
    private GrpcPromisesFactory<Request, Response> responseFactory;

    public ProxyService(ILogger<ProxyService> logger)
    {
        this.logger = logger;
    }

    public override Task OnMessage(IAsyncStreamReader<Response> requestStream, IServerStreamWriter<Request> responseStream, ServerCallContext context)
    {
        logger.LogInformation("Grpc client connected");
        (responseFactory, var readingTask) = GrpcPromisesFactory<Request, Response>.Initialize(responseStream, requestStream, context.CancellationToken);

        return readingTask;
    }

    public Task<GrpcPromiseContext<Response>> ForwardRequest(Request request, CancellationToken cancellationToken)
    {
        if (responseFactory == null) // TODO: what if client disconnects?
        {
            throw new ArgumentNullException("No client connected");
        }

        logger.LogInformation("Request received {Request}", request);

        return responseFactory.SendAndWaitForResonse(request, cancellationToken);
    }
}
