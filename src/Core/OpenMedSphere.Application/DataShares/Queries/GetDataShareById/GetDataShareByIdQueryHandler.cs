using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.DataShares.Queries.GetDataShareById;

/// <summary>
/// Handles the <see cref="GetDataShareByIdQuery"/>.
/// </summary>
internal sealed class GetDataShareByIdQueryHandler(IDataShareRepository repository)
    : IQueryHandler<GetDataShareByIdQuery, DataShareResponse>
{
    /// <inheritdoc />
    public async Task<Result<DataShareResponse>> HandleAsync(
        GetDataShareByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        DataShare? dataShare = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (dataShare is null)
        {
            return Result<DataShareResponse>.NotFound($"Data share with ID '{query.Id}' not found.");
        }

        if (dataShare.SenderResearcherId != query.ResearcherId &&
            dataShare.RecipientResearcherId != query.ResearcherId)
        {
            return Result<DataShareResponse>.NotFound($"Data share with ID '{query.Id}' not found.");
        }

        DataShareResponse response = new()
        {
            Id = dataShare.Id,
            SenderResearcherId = dataShare.SenderResearcherId,
            RecipientResearcherId = dataShare.RecipientResearcherId,
            PatientDataId = dataShare.PatientDataId,
            EncryptedPayload = dataShare.EncryptedPayload,
            EncapsulatedKey = dataShare.EncapsulatedKey,
            Signature = dataShare.Signature,
            SenderKeyVersion = dataShare.SenderKeyVersion,
            RecipientKeyVersion = dataShare.RecipientKeyVersion,
            Status = dataShare.Status,
            SharedAtUtc = dataShare.SharedAtUtc,
            ExpiresAtUtc = dataShare.ExpiresAtUtc
        };

        return Result<DataShareResponse>.Success(response);
    }
}
