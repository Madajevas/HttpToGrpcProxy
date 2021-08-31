using System.Text;

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
    endpoints.Map("/first", context => RedirectToAddress("first", context));
    endpoints.Map("/second", context => RedirectToAddress("second", context));
    endpoints.MapGet("/test", context => context.Response.WriteAsync("test indeed"));
});

app.Run();

async Task RedirectToAddress(string prefix, HttpContext context)
{
    Console.WriteLine("request received");

    var client = context.RequestServices.GetService<IHttpClientFactory>().CreateClient();

    var request = new HttpRequestMessage(HttpMethod.Post, $"http://{prefix}.example.com/{prefix}")
    {
        Content = new StringContent("{\"after_the_rain\": \"there is always sunshine\"}", Encoding.UTF8, "application/json")
    };

    var response = await client.SendAsync(request);

    await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
    await context.Response.CompleteAsync();
}