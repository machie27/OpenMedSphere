var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.OpenMedSphere_API>("openmedsphere-api");

builder.Build().Run();
