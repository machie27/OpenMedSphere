using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.Researchers.Queries.GetResearcherById;

/// <summary>
/// Query to get a researcher by their identifier.
/// </summary>
public sealed record GetResearcherByIdQuery : IQuery<ResearcherResponse>
{
    /// <summary>
    /// Gets the researcher ID.
    /// </summary>
    public required Guid Id { get; init; }
}
