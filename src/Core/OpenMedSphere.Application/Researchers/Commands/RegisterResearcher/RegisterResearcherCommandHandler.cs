using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Application.Researchers.Commands.RegisterResearcher;

/// <summary>
/// Handles the <see cref="RegisterResearcherCommand"/>.
/// </summary>
internal sealed class RegisterResearcherCommandHandler(
    IResearcherRepository repository,
    IUnitOfWork unitOfWork,
    IUniqueConstraintViolationDetector uniqueConstraintDetector)
    : ICommandHandler<RegisterResearcherCommand, Guid>
{
    /// <inheritdoc />
    public async Task<Result<Guid>> HandleAsync(
        RegisterResearcherCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingByExternalId = await repository.GetByExternalIdAsync(command.ExternalId, cancellationToken);

        if (existingByExternalId is not null)
        {
            return Result<Guid>.Conflict("A researcher profile already exists for this identity.");
        }

        var existing = await repository.GetByEmailAsync(command.Email, cancellationToken);

        if (existing is not null)
        {
            return Result<Guid>.Conflict($"A researcher with email '{command.Email}' already exists.");
        }

        var publicKeys = PublicKeySet.Create(
            command.MlKemPublicKey,
            command.MlDsaPublicKey,
            command.X25519PublicKey,
            command.EcdsaPublicKey,
            keyVersion: 1);

        var researcher = Researcher.Create(command.ExternalId, command.Name, command.Email, command.Institution, publicKeys);

        await repository.AddAsync(researcher, cancellationToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (uniqueConstraintDetector.IsUniqueConstraintViolation(ex, ResearcherIndexNames.ExternalIdUnique))
        {
            return Result<Guid>.Conflict("A researcher profile already exists for this identity.");
        }
        catch (Exception ex) when (uniqueConstraintDetector.IsUniqueConstraintViolation(ex, ResearcherIndexNames.EmailUnique))
        {
            return Result<Guid>.Conflict($"A researcher with email '{command.Email}' already exists.");
        }

        return Result<Guid>.Success(researcher.Id);
    }
}
