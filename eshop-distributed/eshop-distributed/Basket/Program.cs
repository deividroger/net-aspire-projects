using Basket.ApiClients;
using Basket.Endpoints;
using Basket.Services;
using ServiceDefaults.Messaging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddKeyedRedisDistributedCache("cache");
builder.Services.AddScoped<BasketService>();
builder.Services.AddMassTransientWithAssemblies(Assembly.GetExecutingAssembly());

builder.Services.AddHttpClient<CatalogApiClient>(client =>
{
    client.BaseAddress = new("http+https://catalog");
});

// Add services to the container.

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapBasketEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();

