using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMedSphere.Application.Abstractions.MedicalTerminology;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Infrastructure.MedicalTerminology;

/// <summary>
/// WHO ICD-11 API implementation of the medical terminology provider.
/// </summary>
internal sealed partial class Icd11TerminologyProvider(
    HttpClient httpClient,
    IMemoryCache cache,
    IOptions<Icd11ApiOptions> options,
    ILogger<Icd11TerminologyProvider> logger) : IMedicalTerminologyProvider
{
    private const string ReleaseId = "2025-01";

    private readonly Icd11ApiOptions _options = options.Value;

    /// <inheritdoc />
    public string CodingSystem => "ICD-11";

    /// <inheritdoc />
    public async Task<IReadOnlyList<MedicalCode>> SearchAsync(
        string searchText,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchText);

        string cacheKey = $"icd11:search:{searchText.ToLowerInvariant()}";

        if (cache.TryGetValue(cacheKey, out IReadOnlyList<MedicalCode>? cached) && cached is not null)
        {
            LogSearchCacheHit(searchText, cached.Count);
            return cached;
        }

        try
        {
            string url = $"/icd/release/11/{ReleaseId}/mms/search?q={Uri.EscapeDataString(searchText)}" +
                         $"&subtreeFilterUsesFoundationDescendants=false&includeKeywordResult=false" +
                         $"&useFlexisearch=false&flatResults=true" +
                         $"&highlightingEnabled=false&medicalCodingMode=true";

            httpClient.DefaultRequestHeaders.Remove("Accept-Language");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", _options.Language);
            httpClient.DefaultRequestHeaders.Remove("API-Version");
            httpClient.DefaultRequestHeaders.Add("API-Version", "v2");

            Icd11SearchResponse? response = await httpClient
                .GetFromJsonAsync<Icd11SearchResponse>(url, cancellationToken);

            if (response?.DestinationEntities is null)
            {
                return [];
            }

            List<MedicalCode> results = response.DestinationEntities
                .Where(e => !string.IsNullOrWhiteSpace(e.TheCode) && !string.IsNullOrWhiteSpace(e.Title))
                .Select(e => MedicalCode.Create(
                    code: e.TheCode!,
                    displayName: StripHtmlTags(e.Title!),
                    codingSystem: CodingSystem,
                    entityUri: e.Id))
                .ToList();

            MemoryCacheEntryOptions cacheEntryOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.CacheDurationMinutes),
                Size = 1
            };
            cache.Set(cacheKey, (IReadOnlyList<MedicalCode>)results.AsReadOnly(), cacheEntryOptions);

            return results.AsReadOnly();
        }
        catch (Exception ex)
        {
            LogSearchFailed(searchText, ex);
            return [];
        }
    }

    /// <inheritdoc />
    public async Task<MedicalCode?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        string cacheKey = $"icd11:code:{code}";

        if (cache.TryGetValue(cacheKey, out MedicalCode? cached))
        {
            LogCodeLookupCacheHit(code);
            return cached;
        }

        try
        {
            string url = $"/icd/release/11/{ReleaseId}/mms/codeinfo/{Uri.EscapeDataString(code)}" +
                         $"?flexiblemode=false";

            httpClient.DefaultRequestHeaders.Remove("Accept-Language");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", _options.Language);
            httpClient.DefaultRequestHeaders.Remove("API-Version");
            httpClient.DefaultRequestHeaders.Add("API-Version", "v2");

            Icd11EntityResponse? response = await httpClient
                .GetFromJsonAsync<Icd11EntityResponse>(url, cancellationToken);

            if (response is null || string.IsNullOrWhiteSpace(response.Title?.Value))
            {
                return null;
            }

            MedicalCode result = MedicalCode.Create(
                code: response.Code ?? code,
                displayName: StripHtmlTags(response.Title.Value),
                codingSystem: CodingSystem,
                entityUri: response.Id);

            MemoryCacheEntryOptions codeCacheOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.CacheDurationMinutes),
                Size = 1
            };
            cache.Set(cacheKey, result, codeCacheOptions);

            return result;
        }
        catch (Exception ex)
        {
            LogCodeLookupFailed(code, ex);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<MedicalCode?> GetByEntityUriAsync(
        string entityUri,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityUri);

        string cacheKey = $"icd11:entity:{entityUri}";

        if (cache.TryGetValue(cacheKey, out MedicalCode? cached))
        {
            LogEntityUriLookupCacheHit(entityUri);
            return cached;
        }

        try
        {
            httpClient.DefaultRequestHeaders.Remove("Accept-Language");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", _options.Language);
            httpClient.DefaultRequestHeaders.Remove("API-Version");
            httpClient.DefaultRequestHeaders.Add("API-Version", "v2");

            Icd11EntityResponse? response = await httpClient
                .GetFromJsonAsync<Icd11EntityResponse>(entityUri, cancellationToken);

            if (response is null || string.IsNullOrWhiteSpace(response.Title?.Value))
            {
                return null;
            }

            MedicalCode result = MedicalCode.Create(
                code: response.Code ?? entityUri,
                displayName: StripHtmlTags(response.Title.Value),
                codingSystem: CodingSystem,
                entityUri: response.Id ?? entityUri);

            MemoryCacheEntryOptions entityCacheOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.CacheDurationMinutes),
                Size = 1
            };
            cache.Set(cacheKey, result, entityCacheOptions);

            return result;
        }
        catch (Exception ex)
        {
            LogEntityUriLookupFailed(entityUri, ex);
            return null;
        }
    }

    private static string StripHtmlTags(string input) =>
        HtmlTagRegex().Replace(input, string.Empty);

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex HtmlTagRegex();

    [LoggerMessage(EventId = 2020, Level = LogLevel.Debug, Message = "ICD-11 search cache hit for '{SearchText}' with {ResultCount} results")]
    private partial void LogSearchCacheHit(string searchText, int resultCount);

    [LoggerMessage(EventId = 2021, Level = LogLevel.Warning, Message = "Failed to search ICD-11 API for '{SearchText}'")]
    private partial void LogSearchFailed(string searchText, Exception ex);

    [LoggerMessage(EventId = 2022, Level = LogLevel.Debug, Message = "ICD-11 code lookup cache hit for '{Code}'")]
    private partial void LogCodeLookupCacheHit(string code);

    [LoggerMessage(EventId = 2023, Level = LogLevel.Warning, Message = "Failed to lookup ICD-11 code '{Code}'")]
    private partial void LogCodeLookupFailed(string code, Exception ex);

    [LoggerMessage(EventId = 2024, Level = LogLevel.Debug, Message = "ICD-11 entity URI lookup cache hit for '{EntityUri}'")]
    private partial void LogEntityUriLookupCacheHit(string entityUri);

    [LoggerMessage(EventId = 2025, Level = LogLevel.Warning, Message = "Failed to lookup ICD-11 entity URI '{EntityUri}'")]
    private partial void LogEntityUriLookupFailed(string entityUri, Exception ex);
}
