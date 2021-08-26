
using HttpToGrpcProxy.Services;

using Microsoft.AspNetCore.Server.Kestrel.Core;

using System.Net;

public class Program
{
    public static WebApplication app;

    public static Task Main(string[] args)
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

        app = builder.Build();



        app.Use(async (context, next) =>
        {

            var routeData = context.GetRouteData();

            var logger = (ILogger<Program>)context.RequestServices.GetRequiredService(typeof(ILogger<Program>));

            logger.LogInformation("test");
            // Do work that doesn't write to the Response.
            await next.Invoke();
            // Do logging or other work that doesn't write to the Response.
        });



        // Configure the HTTP request pipeline.
        if (builder.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        // app.UseAuthorization();

        app.MapControllers().RequireHost($"*:5000");
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            // endpoints.MapGet("/weatherforecast", async context => await context.Response.WriteAsync("Hello World!"));

            // endpoints.Map("/{**route}", async context => await context.Response.WriteAsync(context.Request.RouteValues["route"].ToString())).RequireHost("*:5000");

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // endpoints.MapControllers().RequireHost("*:5000");
        });

        // app.UseEndpoints(endpoints =>
        // {
        //     endpoints.MapControllers();
        // });


        app.MapGrpcService<ProxyService>().RequireHost($"*:6000");

        // await Task.Yield();

        return app.RunAsync();
    }
}
