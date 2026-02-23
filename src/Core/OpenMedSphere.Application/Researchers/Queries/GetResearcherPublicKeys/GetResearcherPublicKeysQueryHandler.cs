using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.Researchers.Queries.GetResearcherPublicKeys;

/// <summary>
/// Handles the <see cref="GetResearcherPublicKeysQuery"/>.
/// </summary>
internal sealed class GetResearcherPublicKeysQueryHandler(IResearcherRepository repository)
    : IQueryHandler<GetResearcherPublicKeysQuery, PublicKeySetResponse>
{
    /// <inheritdoc />
    public async Task<Result<PublicKeySetResponse>> HandleAsync(
        GetResearcherPublicKeysQuery query,
        CancellationToken cancellationToken = default)
    {
        Researcher? researcher = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (researcher is null)
        {
            return Result<PublicKeySetResponse>.NotFound($"Researcher with ID '{query.Id}' not found.");
        }

        PublicKeySetResponse response = new()
        {
            MlKemPublicKey = researcher.PublicKeys.MlKemPublicKey,
            MlDsaPublicKey = researcher.PublicKeys.MlDsaPublicKey,
            X25519PublicKey = researcher.PublicKeys.X25519PublicKey,
            EcdsaPublicKey = researcher.PublicKeys.EcdsaPublicKey,
            KeyVersion = researcher.PublicKeys.KeyVersion
        };

        return Result<PublicKeySetResponse>.Success(response);
    }
}
