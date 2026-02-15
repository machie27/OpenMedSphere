using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenMedSphere.API.Endpoints;
using OpenMedSphere.Application;
using OpenMedSphere.Infrastructure;
using OpenMedSphere.Infrastructure.Persistence;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Paste your JWT token (from POST /api/auth/dev-token)"
        };
        return Task.CompletedTask;
    });
});
builder.AddRedisDistributedCache("cache");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

string jwtKey = builder.Configuration["Jwt:Key"] ?? "OpenMedSphere-Development-Key-That-Is-At-Least-32-Bytes!";
string jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "OpenMedSphere-Dev";
string jwtAudience = builder.Configuration["Jwt:Audience"] ?? "OpenMedSphere-Dev";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("write", limiterOptions =>
    {
        limiterOptions.PermitLimit = 30;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    if (builder.Configuration.GetValue("Database:AutoMigrate", true))
    {
        await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
        {
            ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }

    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.AddHttpAuthentication("Bearer", _ => { });
        options.AddPreferredSecuritySchemes("Bearer");
    });
    app.MapAuthEndpoints();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapPatientDataEndpoints();
app.MapResearchStudyEndpoints();
app.MapAnonymizationPolicyEndpoints();
app.MapMedicalTerminologyEndpoints();

app.Run();
