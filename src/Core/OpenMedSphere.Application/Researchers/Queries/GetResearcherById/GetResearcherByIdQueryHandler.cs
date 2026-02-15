using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.Researchers.Queries.GetResearcherById;

/// <summary>
/// Handles the <see cref="GetResearcherByIdQuery"/>.
/// </summary>
internal sealed class GetResearcherByIdQueryHandler(IResearcherRepository repository)
    : IQueryHandler<GetResearcherByIdQuery, ResearcherResponse>
{
    /// <inheritdoc />
    public async Task<Result<ResearcherResponse>> HandleAsync(
        GetResearcherByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        Researcher? researcher = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (researcher is null)
        {
            return Result<ResearcherResponse>.NotFound($"Researcher with ID '{query.Id}' not found.");
        }

        ResearcherResponse response = new()
        {
            Id = researcher.Id,
            Name = researcher.Name,
            Email = researcher.Email,
            Institution = researcher.Institution,
            KeyVersion = researcher.PublicKeys.KeyVersion,
            IsActive = researcher.IsActive,
            CreatedAtUtc = researcher.CreatedAtUtc
        };

        return Result<ResearcherResponse>.Success(response);
    }
}
