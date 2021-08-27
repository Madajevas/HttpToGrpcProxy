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

        app.UseEndpoints(endpoints => endpoints.Map("{**route}", async context =>
        {
            var proxy = context.RequestServices.GetService<ProxyService>();

            var request = new Request
            {
                Route = context.Request.RouteValues["route"].ToString(),
                Method = context.Request.Method,
                // Body = await new StreamReader(context.Request.Body).ReadToEndAsync() // TODO: if post
            };
            var response = await proxy.ForwardRequest(request);

            // var body = System.Text.Json.JsonSerializer.Serialize(new { Route = context.Request.RouteValues["route"], Method = context.Request.Method, Body = response.Body });
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(response.Response.Body);
            await context.Response.CompleteAsync();
        }));

        app.MapGrpcService<ProxyService>().RequireHost($"*:6000");

        return app;
    }
}
