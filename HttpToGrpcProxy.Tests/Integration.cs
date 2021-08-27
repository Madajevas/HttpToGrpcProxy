using Grpc.Core;
using Grpc.Net.Client;

using Microsoft.Extensions.Hosting;

using NUnit.Framework;

using RestSharp;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpToGrpcProxy.Tests
{
    //public class Integration
    //{
    //    private Task task;
    //    private RestClient httpClient;
    //    private Proxy.ProxyClient grpcClient;
    //    private MyClient grpc;
    //    private AsyncDuplexStreamingCall<Response, Request> handle;

    //    [SetUp]
    //    public async Task SetUp()
    //    {
    //        task =  Program.Main(Array.Empty<string>());
            
    //        httpClient = new RestClient("http://localhost:5000");

    //        var channel = GrpcChannel.ForAddress("http://localhost:6000");
    //        grpcClient = new HttpToGrpcProxy.Proxy.ProxyClient(channel);
            
    //        grpc = new MyClient(grpcClient);
    //    }

    //    [TearDown]
    //    public async Task TearDown()
    //    {
    //        // await Program.app.StopAsync();
    //    }

    //    [Test]
    //    public async Task Test()
    //    {
    //        var tsk = grpc.WaitOnRoute("everything/weatherforecast");

    //        var restRequest = new RestRequest("/everything/weatherforecast", Method.POST);
    //        restRequest.AddJsonBody("testing");
    //        var b = httpClient.ExecuteAsync(restRequest);

   

    //        var requestContext = await tsk;

    //        // Assert.That(requestContext.Request.Method.ToString(), Is.EqualTo("GET"));

    //        await requestContext.Respond(new Response { Body = "i respond" });

    //        var res = await b;

    //        Assert.Pass();
    //    }
    //}

    //class ResponseContext
    //{
    //    private readonly IClientStreamWriter<Response> clientStreamWriter;

    //    public Request Request { get; }

    //    public ResponseContext(Request request, IClientStreamWriter<Response> clientStreamWriter)
    //    {
    //        Request = request;
    //        this.clientStreamWriter = clientStreamWriter;
    //    }

    //    public async Task Respond(Response response)
    //    {
    //        if (string.IsNullOrWhiteSpace(response.Route))
    //        {
    //            response.Route = Request.Route;
    //        }

    //        await clientStreamWriter.WriteAsync(response);
    //    }
    //}

    //class MyClient
    //{
    //    private ConcurrentDictionary<string, TaskCompletionSource<ResponseContext>> promises = new ConcurrentDictionary<string, TaskCompletionSource<ResponseContext>>();

    //    private AsyncDuplexStreamingCall<Response, Request> handle;

    //    public MyClient(Proxy.ProxyClient proxy)
    //    {
    //        handle = proxy.OnMessage();

    //        InitReader(handle.ResponseStream);
    //    }

    //    public Task<ResponseContext> WaitOnRoute(string route)
    //    {
    //        var tsc = new TaskCompletionSource<ResponseContext>();
    //        promises[route] = tsc;

    //        return tsc.Task;
    //    }

    //    private async Task InitReader(IAsyncStreamReader<Request> responseStream/*, ServerCallContext context*/)
    //    {
    //        while (await responseStream.MoveNext()/* && !context.CancellationToken.IsCancellationRequested*/)
    //        {
    //            var message = responseStream.Current;
    //            // logger.LogInformation("Request received {Response}", message);

    //            promises[message.Route].SetResult(new ResponseContext(message, handle.RequestStream));
    //        }
    //    }
    //}
}
