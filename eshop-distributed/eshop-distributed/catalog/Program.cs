using catalog.Services;
using System.Reflection;
using ServiceDefaults.Messaging;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogdb");
builder.Services.AddScoped<ProductService>();
builder.Services.AddMassTransientWithAssemblies(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseMigration();

app.MapProductEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();
