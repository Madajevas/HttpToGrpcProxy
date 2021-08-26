using Grpc.Net.Client;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;


using NUnit.Framework;

namespace HttpToGrpcProxy.Tests;

class ProxyApplication : WebApplicationFactory<Program>
{
    IWebHost _host;
    public string RootUri { get; set; }

    public ProxyApplication() : base()
    {
        ClientOptions.BaseAddress = new Uri("http://localhost:5000");
        // this.Server.BaseAddress = new Uri("http://localhost:5000");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development"); //will be default in RC1
    }

    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        //Real TCP port
        _host = builder.Build();
        _host.Start();
        RootUri = _host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.FirstOrDefault();

        //Fake Server we won't use...sad!
        return new TestServer(new WebHostBuilder().UseStartup<Program>());
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _host.Dispose();
        }
    }
}

public class Tests
{
    private HttpClient client;
    private GrpcChannel channel;
    private Proxy.ProxyClient proxy;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var application = new ProxyApplication();

        // Program.Main(Array.Empty<string>());

        client = application.CreateClient();
        // var response = await client.GetAsync("/todos");

        channel = GrpcChannel.ForAddress("http://localhost:6000");
        proxy = new Proxy.ProxyClient(channel);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        client.Dispose();
        channel.Dispose();
    }

    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public async Task Test1()
    {
        client.BaseAddress = new Uri("http://localhost:5000");

        var a = await client.GetAsync("/testing");

        var handle = proxy.OnMessage();
        await handle.RequestStream.WriteAsync(new Response { Route = "/das/route" });
        // await handle.RequestStream.CompleteAsync();
        // handle.ResponseStream.MoveNext();

        Assert.Pass();
    }
}