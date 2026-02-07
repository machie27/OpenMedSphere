using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Hybrid;
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
    HybridCache cache,
    IOptions<Icd11ApiOptions> options,
    ILogger<Icd11TerminologyProvider> logger) : IMedicalTerminologyProvider
{
    private readonly Icd11ApiOptions _options = options.Value;

    /// <summary>
    /// Wrapper for caching nullable results in HybridCache.
    /// </summary>
    private sealed record CachedCode(MedicalCode? Value);

    /// <inheritdoc />
    public string CodingSystem => "ICD-11";

    /// <inheritdoc />
    public async Task<IReadOnlyList<MedicalCode>> SearchAsync(
        string searchText,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchText);

        var cacheKey = $"icd11:search:{searchText.ToLowerInvariant()}";
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(_options.CacheDurationMinutes),
            LocalCacheExpiration = TimeSpan.FromMinutes(_options.CacheDurationMinutes)
        };

        try
        {
            var results = await cache.GetOrCreateAsync(cacheKey, async ct =>
            {
                LogSearchApiCall(searchText);

                var url = $"/icd/release/11/{_options.ReleaseId}/mms/search?q={Uri.EscapeDataString(searchText)}" +
                          $"&subtreeFilterUsesFoundationDescendants=false&includeKeywordResult=false" +
                          $"&useFlexisearch=false&flatResults=true" +
                          $"&highlightingEnabled=false&medicalCodingMode=true";

                httpClient.DefaultRequestHeaders.Remove("Accept-Language");
                httpClient.DefaultRequestHeaders.Add("Accept-Language", _options.Language);
                httpClient.DefaultRequestHeaders.Remove("API-Version");
                httpClient.DefaultRequestHeaders.Add("API-Version", "v2");

                var response = await httpClient
                    .GetFromJsonAsync<Icd11SearchResponse>(url, ct);

                if (response?.DestinationEntities is null)
                {
                    return Array.Empty<MedicalCode>();
                }

                return response.DestinationEntities
                    .Where(e => !string.IsNullOrWhiteSpace(e.TheCode) && !string.IsNullOrWhiteSpace(e.Title))
                    .Select(e => MedicalCode.Create(
                        code: e.TheCode!,
                        displayName: StripHtmlTags(e.Title!),
                        codingSystem: CodingSystem,
                        entityUri: e.Id))
                    .ToArray();
            }, entryOptions, cancellationToken: cancellationToken);

            return results;
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

        var cacheKey = $"icd11:code:{code}";
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(_options.CacheDurationMinutes),
            LocalCacheExpiration = TimeSpan.FromMinutes(_options.CacheDurationMinutes)
        };

        try
        {
            var cached = await cache.GetOrCreateAsync(cacheKey, async ct =>
            {
                LogCodeLookupApiCall(code);

                var url = $"/icd/release/11/{_options.ReleaseId}/mms/codeinfo/{Uri.EscapeDataString(code)}" +
                          $"?flexiblemode=false";

                httpClient.DefaultRequestHeaders.Remove("Accept-Language");
                httpClient.DefaultRequestHeaders.Add("Accept-Language", _options.Language);
                httpClient.DefaultRequestHeaders.Remove("API-Version");
                httpClient.DefaultRequestHeaders.Add("API-Version", "v2");

                var response = await httpClient
                    .GetFromJsonAsync<Icd11EntityResponse>(url, ct);

                if (response is null || string.IsNullOrWhiteSpace(response.Title?.Value))
                {
                    return new CachedCode(null);
                }

                var result = MedicalCode.Create(
                    code: response.Code ?? code,
                    displayName: StripHtmlTags(response.Title.Value),
                    codingSystem: CodingSystem,
                    entityUri: response.Id);

                return new CachedCode(result);
            }, entryOptions, cancellationToken: cancellationToken);

            return cached.Value;
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

        var cacheKey = $"icd11:entity:{entityUri}";
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(_options.CacheDurationMinutes),
            LocalCacheExpiration = TimeSpan.FromMinutes(_options.CacheDurationMinutes)
        };

        try
        {
            var cached = await cache.GetOrCreateAsync(cacheKey, async ct =>
            {
                LogEntityUriLookupApiCall(entityUri);

                httpClient.DefaultRequestHeaders.Remove("Accept-Language");
                httpClient.DefaultRequestHeaders.Add("Accept-Language", _options.Language);
                httpClient.DefaultRequestHeaders.Remove("API-Version");
                httpClient.DefaultRequestHeaders.Add("API-Version", "v2");

                var response = await httpClient
                    .GetFromJsonAsync<Icd11EntityResponse>(entityUri, ct);

                if (response is null || string.IsNullOrWhiteSpace(response.Title?.Value))
                {
                    return new CachedCode(null);
                }

                var result = MedicalCode.Create(
                    code: response.Code ?? entityUri,
                    displayName: StripHtmlTags(response.Title.Value),
                    codingSystem: CodingSystem,
                    entityUri: response.Id ?? entityUri);

                return new CachedCode(result);
            }, entryOptions, cancellationToken: cancellationToken);

            return cached.Value;
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

    [LoggerMessage(EventId = 2020, Level = LogLevel.Debug, Message = "ICD-11 search cache miss, calling API for '{SearchText}'")]
    private partial void LogSearchApiCall(string searchText);

    [LoggerMessage(EventId = 2021, Level = LogLevel.Warning, Message = "Failed to search ICD-11 API for '{SearchText}'")]
    private partial void LogSearchFailed(string searchText, Exception ex);

    [LoggerMessage(EventId = 2022, Level = LogLevel.Debug, Message = "ICD-11 code lookup cache miss, calling API for '{Code}'")]
    private partial void LogCodeLookupApiCall(string code);

    [LoggerMessage(EventId = 2023, Level = LogLevel.Warning, Message = "Failed to lookup ICD-11 code '{Code}'")]
    private partial void LogCodeLookupFailed(string code, Exception ex);

    [LoggerMessage(EventId = 2024, Level = LogLevel.Debug, Message = "ICD-11 entity URI lookup cache miss, calling API for '{EntityUri}'")]
    private partial void LogEntityUriLookupApiCall(string entityUri);

    [LoggerMessage(EventId = 2025, Level = LogLevel.Warning, Message = "Failed to lookup ICD-11 entity URI '{EntityUri}'")]
    private partial void LogEntityUriLookupFailed(string entityUri, Exception ex);
}
