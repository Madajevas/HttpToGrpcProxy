
using Grpc.Core;

namespace HttpToGrpcProxy.Services;
public class ProxyService : Proxy.ProxyBase
{
    private readonly ILogger<ProxyService> logger;

    public ProxyService(ILogger<ProxyService> logger)
    {
        this.logger = logger;
    }

    public override Task OnMessage(IAsyncStreamReader<Response> requestStream, IServerStreamWriter<Request> responseStream, ServerCallContext context)
    {
        var readerTask = Receive(requestStream, context);

        var writerTask = Send(responseStream);

        return Task.WhenAll(readerTask, writerTask);
    }

    private async Task Send(IAsyncStreamWriter<Request> responseStream)
    {
        while (true)
        {
            await responseStream.WriteAsync(new Request { Route = "/request" });
            await Task.Delay(2000);
        }
    }

    private async Task Receive(IAsyncStreamReader<Response> requestStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
        {
            var message = requestStream.Current;
            logger.LogInformation("Request received {Request}", message);
        }
    }
}
