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
    IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterResearcherCommand, Guid>
{
    /// <inheritdoc />
    public async Task<Result<Guid>> HandleAsync(
        RegisterResearcherCommand command,
        CancellationToken cancellationToken = default)
    {
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

        var researcher = Researcher.Create(command.Name, command.Email, command.Institution, publicKeys);

        await repository.AddAsync(researcher, cancellationToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (
            ex.GetType().Name is "DbUpdateException" &&
            ex.InnerException?.Message?.Contains("IX_Researchers_Email", StringComparison.Ordinal) == true)
        {
            // Unique index violation from concurrent insert — the optimistic check above
            // handles the common case; this catches the rare race condition.
            // DbUpdateException checked by name to avoid an EF Core dependency in Application.
            // IMPORTANT: The index name 'IX_Researchers_Email' must match the name in
            // ResearcherConfiguration. If the index is renamed, update this string to match.
            return Result<Guid>.Conflict($"A researcher with email '{command.Email}' already exists.");
        }

        return Result<Guid>.Success(researcher.Id);
    }
}
