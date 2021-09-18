using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.Map("/first", context => RedirectToAddress("http://first.example.com/first", context));
    endpoints.Map("/second", context => RedirectToAddress("https://second.example.com/second", context));
    endpoints.MapGet("/test", context => context.Response.WriteAsync("test indeed"));
});

app.Run();

async Task RedirectToAddress(string url, HttpContext context)
{
    Console.WriteLine("request received");

    var client = context.RequestServices.GetService<IHttpClientFactory>().CreateClient();

    var request = new HttpRequestMessage(HttpMethod.Post, url)
    {
        Content = new StringContent("{\"after_the_rain\": \"there is always sunshine\"}", Encoding.UTF8, "application/json")
    };

    var response = await client.SendAsync(request);

    await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
    await context.Response.CompleteAsync();
}