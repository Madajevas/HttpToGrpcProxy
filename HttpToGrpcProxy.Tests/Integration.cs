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
    public class Integration
    {
        private Task task;
        private RestClient httpClient;
        private Proxy.ProxyClient grpcClient;
        private MyClient grpc;
        private AsyncDuplexStreamingCall<Response, Request> handle;

        [SetUp]
        public async Task SetUp()
        {
            task =  Program.Main(Array.Empty<string>());
            
            httpClient = new RestClient("http://localhost:5000");

            var channel = GrpcChannel.ForAddress("http://localhost:6000");
            grpcClient = new HttpToGrpcProxy.Proxy.ProxyClient(channel);
            
            grpc = new MyClient(grpcClient);
        }

        [TearDown]
        public async Task TearDown()
        {
            await Program.app.StopAsync();
        }

        [Test]
        public async Task Test()
        {
            var tsk = grpc.WaitOnRoute("everything/weatherforecast");

            var request = new RestRequest("/everything/weatherforecast", Method.GET);

            var b = httpClient.ExecuteAsync(request);

            var a = await tsk;

            // b = httpClient.Execute(request);

            // var a = await httpClient.GetAsync<string>(new RestRequest(new Uri("http://localhost:5001/makaronai")));

            // await Task.Delay(25000);




            Assert.Pass();
        }
    }

    class MyClient
    {
        private ConcurrentDictionary<string, TaskCompletionSource<Request>> promises = new ConcurrentDictionary<string, TaskCompletionSource<Request>>();

        public MyClient(Proxy.ProxyClient proxy)
        {
            var handle = proxy.OnMessage();
            InitReader(handle.ResponseStream);
        }

        public Task<Request> WaitOnRoute(string route)
        {
            var tsc = new TaskCompletionSource<Request>();
            promises[route] = tsc;

            return tsc.Task;
        }

        private async Task InitReader(IAsyncStreamReader<Request> responseStream/*, ServerCallContext context*/)
        {
            while (await responseStream.MoveNext()/* && !context.CancellationToken.IsCancellationRequested*/)
            {
                var message = responseStream.Current;
                // logger.LogInformation("Request received {Response}", message);

                promises[message.Route].SetResult(message);
            }
        }
    }
}
