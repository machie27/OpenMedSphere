using System.Text.Json.Serialization;

namespace OpenMedSphere.Infrastructure.MedicalTerminology;

/// <summary>
/// Response from the ICD-11 search API.
/// </summary>
internal sealed record Icd11SearchResponse
{
    [JsonPropertyName("destinationEntities")]
    public List<Icd11DestinationEntity>? DestinationEntities { get; init; }

    [JsonPropertyName("error")]
    public bool Error { get; init; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; init; }

    [JsonPropertyName("resultChopped")]
    public bool ResultChopped { get; init; }

    [JsonPropertyName("wordSuggestionsChopped")]
    public bool WordSuggestionsChopped { get; init; }
}

/// <summary>
/// A destination entity in the ICD-11 search results.
/// </summary>
internal sealed record Icd11DestinationEntity
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("theCode")]
    public string? TheCode { get; init; }

    [JsonPropertyName("score")]
    public double Score { get; init; }

    [JsonPropertyName("titleIsASearchResult")]
    public bool TitleIsASearchResult { get; init; }

    [JsonPropertyName("titleIsTopBlock")]
    public bool TitleIsTopBlock { get; init; }

    [JsonPropertyName("entityType")]
    public int EntityType { get; init; }

    [JsonPropertyName("chapter")]
    public string? Chapter { get; init; }
}
