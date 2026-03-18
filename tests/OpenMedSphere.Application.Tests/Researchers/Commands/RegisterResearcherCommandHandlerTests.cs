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
        private readonly Mock<IUniqueConstraintViolationDetector> _uniqueConstraintDetectorMock;
        private readonly RegisterResearcherCommandHandler _handler;

        public RegisterResearcherCommandHandlerTests()
        {
            _repositoryMock = new Mock<IResearcherRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _uniqueConstraintDetectorMock = new Mock<IUniqueConstraintViolationDetector>();

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _handler = new RegisterResearcherCommandHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object,
                _uniqueConstraintDetectorMock.Object);
        }

        private static RegisterResearcherCommand CreateValidCommand() => new()
        {
            ExternalId = "ext-1",
            Name = "Dr. Smith",
            Email = "smith@university.edu",
            Institution = "MIT",
            MlKemPublicKey = "mlKemKey",
            MlDsaPublicKey = "mlDsaKey",
            X25519PublicKey = "x25519Key",
            EcdsaPublicKey = "ecdsaKey"
        };

        [Fact]
        public async Task HandleAsync_ValidCommand_ReturnsSuccessWithGuid()
        {
            _repositoryMock
                .Setup(r => r.GetByExternalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            _repositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            Result<Guid> result = await _handler.HandleAsync(CreateValidCommand(), CancellationToken.None);

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
        public async Task HandleAsync_DuplicateExternalId_ReturnsConflict()
        {
            var existingResearcher = Researcher.Create(
                "ext-1", "Existing", "existing@university.edu", "MIT",
                Domain.ValueObjects.PublicKeySet.Create("k1", "k2", "k3", "k4", 1));

            _repositoryMock
                .Setup(r => r.GetByExternalIdAsync("ext-1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingResearcher);

            Result<Guid> result = await _handler.HandleAsync(CreateValidCommand(), CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.Conflict, result.ErrorCode);
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Researcher>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_DuplicateEmail_ReturnsConflict()
        {
            _repositoryMock
                .Setup(r => r.GetByExternalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            var existingResearcher = Researcher.Create(
                "ext-other", "Existing", "smith@university.edu", "MIT",
                Domain.ValueObjects.PublicKeySet.Create("k1", "k2", "k3", "k4", 1));

            _repositoryMock
                .Setup(r => r.GetByEmailAsync("smith@university.edu", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingResearcher);

            Result<Guid> result = await _handler.HandleAsync(CreateValidCommand(), CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.Conflict, result.ErrorCode);
            Assert.Contains("smith@university.edu", result.Error!);
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Researcher>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ConcurrentDuplicateExternalId_ReturnsConflict()
        {
            var savedException = new InvalidOperationException("unique constraint violation");

            _repositoryMock
                .Setup(r => r.GetByExternalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            _repositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(savedException);

            _uniqueConstraintDetectorMock
                .Setup(d => d.IsUniqueConstraintViolation(savedException, ResearcherIndexNames.ExternalIdUnique))
                .Returns(true);

            Result<Guid> result = await _handler.HandleAsync(CreateValidCommand(), CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.Conflict, result.ErrorCode);
        }

        [Fact]
        public async Task HandleAsync_ConcurrentDuplicateEmail_ReturnsConflict()
        {
            var savedException = new InvalidOperationException("unique constraint violation");

            _repositoryMock
                .Setup(r => r.GetByExternalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            _repositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(savedException);

            _uniqueConstraintDetectorMock
                .Setup(d => d.IsUniqueConstraintViolation(savedException, ResearcherIndexNames.EmailUnique))
                .Returns(true);

            Result<Guid> result = await _handler.HandleAsync(CreateValidCommand(), CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.Conflict, result.ErrorCode);
            Assert.Contains("smith@university.edu", result.Error!);
        }
    }
}
