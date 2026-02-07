using System.Text.Json.Serialization;

namespace OpenMedSphere.Infrastructure.MedicalTerminology;

/// <summary>
/// Response from the ICD-11 entity/code lookup API.
/// </summary>
internal sealed record Icd11EntityResponse
{
    [JsonPropertyName("@context")]
    public string? Context { get; init; }

    [JsonPropertyName("@id")]
    public string? Id { get; init; }

    [JsonPropertyName("title")]
    public Icd11LanguageValue? Title { get; init; }

    [JsonPropertyName("definition")]
    public Icd11LanguageValue? Definition { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("codeRange")]
    public string? CodeRange { get; init; }

    [JsonPropertyName("classKind")]
    public string? ClassKind { get; init; }

    [JsonPropertyName("parent")]
    public List<string>? Parent { get; init; }

    [JsonPropertyName("child")]
    public List<string>? Child { get; init; }
}

/// <summary>
/// Represents a language-specific value from the ICD-11 API.
/// </summary>
internal sealed record Icd11LanguageValue
{
    [JsonPropertyName("@language")]
    public string? Language { get; init; }

    [JsonPropertyName("@value")]
    public string? Value { get; init; }
}
