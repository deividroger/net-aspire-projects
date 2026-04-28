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


builder.Services.AddHttpClient<CatalogApiClient>(client =>
{
    client.BaseAddress = new("http+https://catalog");
});


builder.Services.AddMassTransientWithAssemblies(Assembly.GetExecutingAssembly());

builder.Services.AddAuthentication()
                .AddKeycloakJwtBearer(serviceName: "keycloak", realm: "eshop", configureOptions: options =>
                {
                    options.Audience = "account";
                    options.RequireHttpsMetadata = false;
                });
builder.Services.AddAuthorization();


// Add services to the container.

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapBasketEndpoints();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();

