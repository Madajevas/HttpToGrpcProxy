
using Grpc.Core;

using System.Collections.Concurrent;

namespace HttpToGrpcProxy.Services;

/// <summary>
/// Intenden for single client
/// </summary>
class ProxyService : Proxy.ProxyBase
{
    private readonly ILogger<ProxyService> logger;

    private IAsyncStreamWriter<Request> responseStream;

    private ConcurrentDictionary<string, TaskCompletionSource<Response>> promises = new ConcurrentDictionary<string, TaskCompletionSource<Response>>();

    public ProxyService(ILogger<ProxyService> logger)
    {
        this.logger = logger;
    }

    public override Task OnMessage(IAsyncStreamReader<Response> requestStream, IServerStreamWriter<Request> responseStream, ServerCallContext context)
    {
        this.responseStream = responseStream;

        return InitReader(requestStream, context);
    }

    public async Task<Response> ForwardRequest(Request request)
    {
        if (responseStream == null) // TODO: what if client disconnects?
        {
            throw new ArgumentNullException("No client connected");
        }

        logger.LogInformation("Request received {Request}", request);

        var tsc = new TaskCompletionSource<Response>();
        promises[request.Route] = tsc;

        await responseStream.WriteAsync(request);

        return await tsc.Task;
    }

    private async Task InitReader(IAsyncStreamReader<Response> requestStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
        {
            var message = requestStream.Current;
            logger.LogInformation("Response received {Response}", message);

            EnsureWaitingForResponse(message.Route).SetResult(message);
        }
    }

    private TaskCompletionSource<Response> EnsureWaitingForResponse(string route)
    {
        if (!promises.ContainsKey(route))
        {
            // TODO: used promises should be discarded
            promises[route] = new TaskCompletionSource<Response>();
        }

        return promises[route];
    }
}
