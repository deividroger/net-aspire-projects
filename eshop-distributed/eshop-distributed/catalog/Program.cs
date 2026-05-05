using catalog.Services;
using Catalog.Models;
using Microsoft.SemanticKernel;
using ServiceDefaults.Messaging;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogdb");
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductAIService>();
builder.Services.AddMassTransientWithAssemblies(Assembly.GetExecutingAssembly());


builder.Services.AddHttpClient("ollama-llama3-2", client =>
{
    client.Timeout = TimeSpan.FromMinutes(2);
});

builder.AddOllamaSharpChatClient("ollama", options =>
{
    options.SelectedModel = "llama3.2";

});

builder.AddOllamaSharpEmbeddingGenerator("ollama", options =>
{
    options.SelectedModel = "all-minilm";
});

builder.Services.AddInMemoryVectorStoreRecordCollection<int, ProductVector>("products");

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseMigration();

app.MapProductEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();