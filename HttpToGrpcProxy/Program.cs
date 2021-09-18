using HttpToGrpcProxy;
using HttpToGrpcProxy.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

public static class Program
{
    private static ILogger logger;

    public static Task Main(string[] args)
    {
        logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger("Program");

        return CreateBuilder(args).CreateApplication().RunAsync();
    }

    public static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseKestrel()
            .ConfigureKestrel((context, options) =>
            {
                // grpc
                options.Listen(IPAddress.Any, 6000, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);

                // http/rest
                var httpPorts = Environment.GetEnvironmentVariable("HTTP_PORTS")?.Split(',').Select(int.Parse) ?? new[] { 5000 };
                foreach (var port in httpPorts)
                {
                    options.Listen(IPAddress.Any, port, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
                }

                // https/rest
                var httpsPorts = Environment.GetEnvironmentVariable("HTTPS_PORTS")?.Split(',').Select(int.Parse) ?? Array.Empty<int>();
                foreach (var port in httpsPorts)
                {
                    options.Listen(IPAddress.Any, port, listenOptions => {
                        listenOptions.Protocols = HttpProtocols.Http1;
                        listenOptions.UseHttps("/app/https.pfx", Environment.GetEnvironmentVariable("HTTPS_CERTIFICATE_PASSWORD"));
                    });
                }
            });

        // Add services to the container.
        builder.Services.AddSingleton<ProxyService>();
        builder.Services.AddGrpc();

        builder.Services.AddRouting();
        builder.Services.AddControllers();

        return builder;
    }

    public static WebApplication CreateApplication(this WebApplicationBuilder builder)
    {
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (builder.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints => endpoints.Map("{**route}", HandleRequest)/*.RequireHost($"*:5000")*/);

        app.MapGrpcService<ProxyService>().RequireHost($"*:6000");

        return app;
    }

    private static async Task HandleRequest(HttpContext context, CancellationToken cancellationToken)
    {
        logger?.LogInformation("Request received {Path}", context.Request.Path);

        var proxy = context.RequestServices.GetService<ProxyService>();

        var request = await GetRequest(context.Request);
        using var response = await proxy.ForwardRequest(request, cancellationToken);

        AddHeaders(response.Value, context.Response);
        await context.Response.WriteAsync(response.Value.Body);
        await context.Response.CompleteAsync();
    }

    private static void AddHeaders(Response response, HttpResponse httpResonse)
    {
        httpResonse.ContentType = response.ContentType;

        if (response.Headers == null)
        {
            return;
        }

        foreach (var header in response.Headers)
        {
            httpResonse.Headers.Add(header.Key, header.Value);
        }
    }

    private static async Task<Request> GetRequest(HttpRequest httpRequest)
    {
        var request = new Request
        {
            Route = httpRequest.RouteValues["route"].ToString(),
            Method = httpRequest.Method,
            Body = await GetBody(httpRequest),
            ContentType = httpRequest.ContentType ?? ""
        };

        request.Headers.Add(httpRequest.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
        request.Query.Add(httpRequest.Query.ToDictionary(q => q.Key, q => q.Value.ToString()));

        return request;
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
