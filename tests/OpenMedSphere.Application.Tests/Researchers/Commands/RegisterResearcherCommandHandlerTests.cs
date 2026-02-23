using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Researchers.Commands.RegisterResearcher;
using OpenMedSphere.Domain.Entities;
using Xunit;

namespace OpenMedSphere.Application.Tests.Researchers.Commands
{
    public sealed class RegisterResearcherCommandHandlerTests
    {
        private readonly Mock<IResearcherRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RegisterResearcherCommandHandler _handler;

        public RegisterResearcherCommandHandlerTests()
        {
            _repositoryMock = new Mock<IResearcherRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _handler = new RegisterResearcherCommandHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ValidCommand_ReturnsSuccessWithGuid()
        {
            _repositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            RegisterResearcherCommand command = new()
            {
                Name = "Dr. Smith",
                Email = "smith@university.edu",
                Institution = "MIT",
                MlKemPublicKey = "mlKemKey",
                MlDsaPublicKey = "mlDsaKey",
                X25519PublicKey = "x25519Key",
                EcdsaPublicKey = "ecdsaKey"
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Researcher>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_DuplicateEmail_ReturnsConflict()
        {
            var existingResearcher = Researcher.Create(
                "Existing", "smith@university.edu", "MIT",
                Domain.ValueObjects.PublicKeySet.Create("k1", "k2", "k3", "k4", 1));

            _repositoryMock
                .Setup(r => r.GetByEmailAsync("smith@university.edu", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingResearcher);

            RegisterResearcherCommand command = new()
            {
                Name = "Dr. Smith",
                Email = "smith@university.edu",
                Institution = "MIT",
                MlKemPublicKey = "mlKemKey",
                MlDsaPublicKey = "mlDsaKey",
                X25519PublicKey = "x25519Key",
                EcdsaPublicKey = "ecdsaKey"
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.Conflict, result.ErrorCode);
            Assert.Contains("smith@university.edu", result.Error!);
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Researcher>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
