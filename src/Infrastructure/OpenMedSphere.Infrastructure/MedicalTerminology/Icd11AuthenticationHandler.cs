using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace OpenMedSphere.Infrastructure.MedicalTerminology;

/// <summary>
/// HTTP delegating handler that obtains and caches OAuth2 bearer tokens for the ICD-11 API.
/// </summary>
internal sealed class Icd11AuthenticationHandler(
    IOptions<Icd11ApiOptions> options,
    IHttpClientFactory httpClientFactory) : DelegatingHandler, IAsyncDisposable
{
    private readonly Icd11ApiOptions _options = options.Value;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string token = await GetTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (_cachedToken is not null && DateTime.UtcNow < _tokenExpiry)
        {
            return _cachedToken;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_cachedToken is not null && DateTime.UtcNow < _tokenExpiry)
            {
                return _cachedToken;
            }

            using var tokenClient = _httpClientFactory.CreateClient();

            List<KeyValuePair<string, string>> formData =
            [
                new("client_id", _options.ClientId!),
                new("client_secret", _options.ClientSecret!),
                new("scope", "icdapi_access"),
                new("grant_type", "client_credentials")
            ];

            using FormUrlEncodedContent content = new(formData);
            HttpResponseMessage response = await tokenClient.PostAsync(
                _options.TokenEndpoint,
                content,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            TokenResponse? tokenResponse = await response.Content
                .ReadFromJsonAsync<TokenResponse>(cancellationToken);

            _cachedToken = tokenResponse?.AccessToken
                ?? throw new InvalidOperationException("Failed to obtain access token from ICD-11 API.");
            _tokenExpiry = DateTime.UtcNow.AddSeconds((tokenResponse.ExpiresIn ?? 3600) - 60);

            return _cachedToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _semaphore.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Asynchronously disposes the handler and its resources.
    /// </summary>
    public ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
