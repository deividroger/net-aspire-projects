var builder = DistributedApplication.CreateBuilder(args);

//Backing services
var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin()
                      .WithDataVolume()
                      .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalogdb");

var cache = builder.AddRedis("cache")
                    .WithRedisInsight()
                    .WithDataVolume()
                    .WithLifetime(ContainerLifetime.Persistent);

var rabbitMQ = builder.AddRabbitMQ("rabbitmq")
                      .WithManagementPlugin()
                      .WithDataVolume()
                      .WithLifetime(ContainerLifetime.Persistent);

//Projects
var catalog = builder.AddProject<Projects.catalog>("catalog")
    .WithReference(catalogDb)
    .WithReference(rabbitMQ)
    .WaitFor(catalogDb)
    .WaitFor(rabbitMQ);

var basket = builder.AddProject<Projects.Basket>("basket")
    .WithReference(cache)
    .WithReference(catalog)
    .WithReference(rabbitMQ)
    .WaitFor(cache)
    .WaitFor(rabbitMQ);

builder.Build().Run();