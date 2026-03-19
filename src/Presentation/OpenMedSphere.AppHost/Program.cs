var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var database = postgres.AddDatabase("openmedsphere-db");

var cache = builder.AddRedis("cache");

var api = builder.AddProject<Projects.OpenMedSphere_API>("openmedsphere-api")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WaitFor(cache)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
