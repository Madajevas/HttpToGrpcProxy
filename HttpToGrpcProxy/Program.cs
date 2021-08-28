using HttpToGrpcProxy;
using HttpToGrpcProxy.Services;

using Microsoft.AspNetCore.Server.Kestrel.Core;

using System.Net;

public class Program
{
    public static Task Main(string[] args)
    {
        return CreateApplication(args).RunAsync();
    }

    public static WebApplication CreateApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseKestrel()
            .ConfigureKestrel((context, options) =>
            {
                // grpc
                options.Listen(IPAddress.Any, 6000, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                // http/rest
                options.Listen(IPAddress.Any, 5000, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
            });

        // Add services to the container.
        builder.Services.AddSingleton<ProxyService>();
        builder.Services.AddGrpc();

        builder.Services.AddRouting();
        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (builder.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints => endpoints.Map("{**route}", HandleRequest));

        app.MapGrpcService<ProxyService>().RequireHost($"*:6000");

        return app;
    }

    private static async Task HandleRequest(HttpContext context)
    {
        var proxy = context.RequestServices.GetService<ProxyService>();

        var request = new Request
        {
            Route = context.Request.RouteValues["route"].ToString(),
            Method = context.Request.Method,
            Body = await GetBody(context.Request),
            ContentType = context.Request.ContentType ?? ""
        };
        using var response = await proxy.ForwardRequest(request);

        context.Response.ContentType = response.Value.ContentType;
        await context.Response.WriteAsync(response.Value.Body);
        await context.Response.CompleteAsync();
    }

    private static async Task<string?> GetBody(HttpRequest request)
    {
        return request.Method switch
        {
            HttpMethods.Post or HttpMethods.Put or HttpMethods.Patch => await new StreamReader(request.Body).ReadToEndAsync(),
            HttpMethods.Get or HttpMethods.Delete or HttpMethods.Head => "", // at the moment protobuf optional is not supported
            _ => throw new InvalidOperationException($"Method {request.Method} is not supported")
        };
    }

    class HttpMethods
    {
        public const string Get = "GET";
        public const string Post = "POST";
        public const string Put = "PUT";
        public const string Delete = "DELETE";
        public const string Patch = "PATCH";
        public const string Head = "HEAD";
    }
}
