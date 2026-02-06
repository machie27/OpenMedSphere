using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Abstractions.MedicalTerminology;
using OpenMedSphere.Infrastructure.MedicalTerminology;
using OpenMedSphere.Infrastructure.Persistence;
using OpenMedSphere.Infrastructure.Persistence.Interceptors;
using OpenMedSphere.Infrastructure.Persistence.Repositories;

namespace OpenMedSphere.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddMedicalTerminology(services, configuration);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("openmedsphere-db");

        services.AddSingleton<AuditSaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IPatientDataRepository, PatientDataRepository>();
        services.AddScoped<IResearchStudyRepository, ResearchStudyRepository>();
        services.AddScoped<IAnonymizationPolicyRepository, AnonymizationPolicyRepository>();
    }

    private static void AddMedicalTerminology(IServiceCollection services, IConfiguration configuration)
    {
        Icd11ApiOptions icd11Options = new();
        configuration.GetSection(Icd11ApiOptions.SectionName).Bind(icd11Options);

        services.Configure<Icd11ApiOptions>(configuration.GetSection(Icd11ApiOptions.SectionName));

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new Microsoft.Extensions.Caching.Hybrid.HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(icd11Options.CacheDurationMinutes),
                LocalCacheExpiration = TimeSpan.FromMinutes(icd11Options.CacheDurationMinutes)
            };
        });

        // Always register the fallback provider for baseline ICD-11 lookup
        services.AddSingleton<IMedicalTerminologyProvider, FallbackTerminologyProvider>();

        if (icd11Options.IsConfigured)
        {
            // When ICD-11 API is configured, also register the API-backed provider
            services.AddTransient<Icd11AuthenticationHandler>();

            services.AddHttpClient<IMedicalTerminologyProvider, Icd11TerminologyProvider>(client =>
            {
                client.BaseAddress = new Uri(icd11Options.BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<Icd11AuthenticationHandler>();
        }

        // Composite service aggregates all registered providers
        services.AddScoped<IMedicalTerminologyService, MedicalTerminologyService>();
    }
}
