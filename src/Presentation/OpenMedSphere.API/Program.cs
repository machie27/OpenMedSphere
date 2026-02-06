using Microsoft.EntityFrameworkCore;
using OpenMedSphere.API.Endpoints;
using OpenMedSphere.Application;
using OpenMedSphere.Infrastructure;
using OpenMedSphere.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
    {
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapPatientDataEndpoints();
app.MapResearchStudyEndpoints();
app.MapAnonymizationPolicyEndpoints();
app.MapMedicalTerminologyEndpoints();

app.Run();
