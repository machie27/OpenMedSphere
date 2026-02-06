using Microsoft.Extensions.Logging;
using OpenMedSphere.Application.Abstractions.MedicalTerminology;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Infrastructure.MedicalTerminology;

/// <summary>
/// Composite medical terminology service that delegates to all registered
/// <see cref="IMedicalTerminologyProvider"/> instances.
/// </summary>
internal sealed partial class MedicalTerminologyService(
    IEnumerable<IMedicalTerminologyProvider> providers,
    ILogger<MedicalTerminologyService> logger) : IMedicalTerminologyService
{
    private readonly IReadOnlyList<IMedicalTerminologyProvider> _providers = providers.ToList().AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyList<string> GetSupportedCodingSystems()
    {
        IReadOnlyList<string> systems = _providers
            .Select(p => p.CodingSystem)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList()
            .AsReadOnly();

        LogGetSupportedCodingSystems(systems.Count);

        return systems;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MedicalCode>> SearchAsync(
        string searchText,
        string? codingSystem = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchText);

        IEnumerable<IMedicalTerminologyProvider> selectedProviders = GetProviders(codingSystem);
        List<MedicalCode> results = [];

        foreach (IMedicalTerminologyProvider provider in selectedProviders)
        {
            LogSearchRouting(provider.CodingSystem, searchText);
            IReadOnlyList<MedicalCode> providerResults =
                await provider.SearchAsync(searchText, cancellationToken);
            results.AddRange(providerResults);
        }

        LogSearchCompleted(searchText, codingSystem, results.Count);

        return results.AsReadOnly();
    }

    /// <inheritdoc />
    public async Task<MedicalCode?> GetByCodeAsync(
        string code,
        string? codingSystem = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        IEnumerable<IMedicalTerminologyProvider> selectedProviders = GetProviders(codingSystem);

        foreach (IMedicalTerminologyProvider provider in selectedProviders)
        {
            LogCodeLookupRouting(provider.CodingSystem, code);
            MedicalCode? result = await provider.GetByCodeAsync(code, cancellationToken);
            if (result is not null)
            {
                LogCodeLookupFound(code, provider.CodingSystem);
                return result;
            }
        }

        return null;
    }

    /// <inheritdoc />
    public async Task<MedicalCode?> GetByEntityUriAsync(
        string entityUri,
        string? codingSystem = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityUri);

        IEnumerable<IMedicalTerminologyProvider> selectedProviders = GetProviders(codingSystem);

        foreach (IMedicalTerminologyProvider provider in selectedProviders)
        {
            MedicalCode? result = await provider.GetByEntityUriAsync(entityUri, cancellationToken);
            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }

    private IEnumerable<IMedicalTerminologyProvider> GetProviders(string? codingSystem) =>
        codingSystem is null
            ? _providers
            : _providers.Where(p => p.CodingSystem.Equals(codingSystem, StringComparison.OrdinalIgnoreCase));

    [LoggerMessage(EventId = 2000, Level = LogLevel.Debug, Message = "Returning {Count} supported coding systems")]
    private partial void LogGetSupportedCodingSystems(int count);

    [LoggerMessage(EventId = 2001, Level = LogLevel.Debug, Message = "Routing search to provider '{CodingSystem}' for '{SearchText}'")]
    private partial void LogSearchRouting(string codingSystem, string searchText);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Debug, Message = "Search for '{SearchText}' (system: {CodingSystem}) returned {ResultCount} total results")]
    private partial void LogSearchCompleted(string searchText, string? codingSystem, int resultCount);

    [LoggerMessage(EventId = 2003, Level = LogLevel.Debug, Message = "Routing code lookup to provider '{CodingSystem}' for '{Code}'")]
    private partial void LogCodeLookupRouting(string codingSystem, string code);

    [LoggerMessage(EventId = 2004, Level = LogLevel.Debug, Message = "Code '{Code}' found via provider '{CodingSystem}'")]
    private partial void LogCodeLookupFound(string code, string codingSystem);

    [LoggerMessage(EventId = 2005, Level = LogLevel.Debug, Message = "Entity URI lookup completed for '{EntityUri}' via provider '{CodingSystem}'")]
    private partial void LogEntityUriLookupCompleted(string entityUri, string codingSystem);
}
