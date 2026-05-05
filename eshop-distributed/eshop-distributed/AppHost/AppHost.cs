var builder = DistributedApplication.CreateBuilder(args);

//Backing services
var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin()
                      //.WithDataVolume()
                      .WithLifetime(ContainerLifetime.Persistent);



var catalogDb = postgres.AddDatabase("catalogdb");

var cache = builder.AddRedis("cache")
                    .WithRedisInsight()
                    //.WithDataVolume()
                    .WithLifetime(ContainerLifetime.Persistent);

var rabbitMQ = builder.AddRabbitMQ("rabbitmq")
                      .WithManagementPlugin()
                      //.WithDataVolume()
                      .WithLifetime(ContainerLifetime.Persistent);

var keycloak = builder.AddKeycloak("keycloak", 8080)
                      //.WithDataVolume()
                      .WithLifetime(ContainerLifetime.Persistent);

var ollama = builder.AddOllama("ollama", 11434)
                      .WithDataVolume()
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithOpenWebUI();

var llama = ollama.AddModel("llama3.2");
var embedding = ollama.AddModel("all-minilm");

if (builder.ExecutionContext.IsRunMode)
{
    postgres.WithDataVolume();
    cache.WithDataVolume();
    rabbitMQ.WithDataVolume();
    keycloak.WithDataVolume();
}

//Projects
var catalog = builder.AddProject<Projects.catalog>("catalog")
    .WithReference(catalogDb)
    .WithReference(rabbitMQ)
    .WithReference(ollama)
    .WithReference(embedding)
    .WaitFor(catalogDb)
    .WaitFor(rabbitMQ)
    .WaitFor(llama)
    .WaitFor(embedding);

var basket = builder.AddProject<Projects.Basket>("basket")
    .WithReference(cache)
    .WithReference(catalog)
    .WithReference(rabbitMQ)
    .WithReference(keycloak)
    .WaitFor(cache)
    .WaitFor(rabbitMQ)
    .WaitFor(keycloak);

builder.AddProject<Projects.WebApp>("webapp")
       .WithExternalHttpEndpoints()
       .WithReference(catalog)
       .WithReference(basket)
       .WithReference(cache)
       .WaitFor(cache)
       .WaitFor(catalog)
       .WaitFor(basket);

builder.Build().Run();