var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.OpenMedSphere_API>("openmedsphere-api")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.OpenMedSphere_Web>("openmedsphere-frontend")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);


builder.Build().Run();
