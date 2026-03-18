using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Researchers.Commands.UpdateResearcherPublicKeys;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Application.Tests.Researchers.Commands
{
    public sealed class UpdateResearcherPublicKeysCommandHandlerTests
    {
        private readonly Mock<IResearcherRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IConcurrencyConflictDetector> _concurrencyDetectorMock;
        private readonly UpdateResearcherPublicKeysCommandHandler _handler;

        public UpdateResearcherPublicKeysCommandHandlerTests()
        {
            _repositoryMock = new Mock<IResearcherRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _concurrencyDetectorMock = new Mock<IConcurrencyConflictDetector>();

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _handler = new UpdateResearcherPublicKeysCommandHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object,
                _concurrencyDetectorMock.Object);
        }

        private static Researcher CreateResearcherWithKeyVersion(int keyVersion)
        {
            var keys = PublicKeySet.Create("k1", "k2", "k3", "k4", keyVersion);
            return Researcher.Create("Dr. Smith", "smith@test.com", "MIT", keys);
        }

        [Fact]
        public async Task HandleAsync_ValidKeyRotation_ReturnsSuccess()
        {
            Researcher researcher = CreateResearcherWithKeyVersion(1);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(researcher.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(researcher);

            UpdateResearcherPublicKeysCommand command = new()
            {
                ResearcherId = researcher.Id,
                MlKemPublicKey = "newK1",
                MlDsaPublicKey = "newK2",
                X25519PublicKey = "newK3",
                EcdsaPublicKey = "newK4",
                KeyVersion = 2
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, researcher.PublicKeys.KeyVersion);
            _repositoryMock.Verify(r => r.Update(researcher), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ResearcherNotFound_ReturnsNotFound()
        {
            Guid researcherId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(researcherId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            UpdateResearcherPublicKeysCommand command = new()
            {
                ResearcherId = researcherId,
                MlKemPublicKey = "k1",
                MlDsaPublicKey = "k2",
                X25519PublicKey = "k3",
                EcdsaPublicKey = "k4",
                KeyVersion = 2
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
        }

        [Fact]
        public async Task HandleAsync_SameKeyVersion_ReturnsInvalidOperation()
        {
            Researcher researcher = CreateResearcherWithKeyVersion(3);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(researcher.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(researcher);

            UpdateResearcherPublicKeysCommand command = new()
            {
                ResearcherId = researcher.Id,
                MlKemPublicKey = "k1",
                MlDsaPublicKey = "k2",
                X25519PublicKey = "k3",
                EcdsaPublicKey = "k4",
                KeyVersion = 3
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.InvalidOperation, result.ErrorCode);
            Assert.Contains("must be greater than the current version", result.Error!);
        }

        [Fact]
        public async Task HandleAsync_InactiveResearcher_ReturnsInvalidOperation()
        {
            Researcher researcher = CreateResearcherWithKeyVersion(1);
            researcher.Deactivate();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(researcher.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(researcher);

            UpdateResearcherPublicKeysCommand command = new()
            {
                ResearcherId = researcher.Id,
                MlKemPublicKey = "k1",
                MlDsaPublicKey = "k2",
                X25519PublicKey = "k3",
                EcdsaPublicKey = "k4",
                KeyVersion = 2
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.InvalidOperation, result.ErrorCode);
            Assert.Contains("inactive", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task HandleAsync_LowerKeyVersion_ReturnsInvalidOperation()
        {
            Researcher researcher = CreateResearcherWithKeyVersion(5);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(researcher.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(researcher);

            UpdateResearcherPublicKeysCommand command = new()
            {
                ResearcherId = researcher.Id,
                MlKemPublicKey = "k1",
                MlDsaPublicKey = "k2",
                X25519PublicKey = "k3",
                EcdsaPublicKey = "k4",
                KeyVersion = 2
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.InvalidOperation, result.ErrorCode);
        }
    }
}
