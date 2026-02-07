namespace OpenMedSphere.Infrastructure.MedicalTerminology;

/// <summary>
/// Configuration options for the WHO ICD-11 API.
/// </summary>
public sealed class Icd11ApiOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "MedicalTerminology:Icd11";

    /// <summary>
    /// Gets or sets the OAuth2 client ID.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the OAuth2 client secret.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the OAuth2 token endpoint.
    /// </summary>
    public string TokenEndpoint { get; set; } = "https://icdaccessmanagement.who.int/connect/token";

    /// <summary>
    /// Gets or sets the ICD-11 API base URL.
    /// </summary>
    public string BaseUrl { get; set; } = "https://id.who.int";

    /// <summary>
    /// Gets or sets the language for API responses.
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Gets or sets the ICD-11 release identifier (e.g. "2025-01").
    /// </summary>
    public string ReleaseId { get; set; } = "2025-01";

    /// <summary>
    /// Gets or sets the cache duration in minutes.
    /// </summary>
    public int CacheDurationMinutes { get; set; } = 60;

    /// <summary>
    /// Gets a value indicating whether the API is configured with credentials.
    /// </summary>
    public bool IsConfigured => !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret);
}
