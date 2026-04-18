using catalog.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogdb");
builder.Services.AddScoped<ProductService>();
var app = builder.Build();

app.MapDefaultEndpoints();

app.UseMigration();

app.MapProductEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();
