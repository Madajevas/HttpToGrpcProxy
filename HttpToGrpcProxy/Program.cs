
using HttpToGrpcProxy.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ProxyService>();
builder.Services.AddGrpc();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();
app.UseRouting();
app.MapGrpcService<ProxyService>();

app.Run();
