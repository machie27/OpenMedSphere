using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.DataShares.Commands.CreateDataShare;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Application.Tests.DataShares.Commands
{
    public sealed class CreateDataShareCommandHandlerTests
    {
        private readonly Mock<IDataShareRepository> _dataShareRepositoryMock;
        private readonly Mock<IResearcherRepository> _researcherRepositoryMock;
        private readonly Mock<IPatientDataRepository> _patientDataRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateDataShareCommandHandler _handler;

        private static readonly Guid SenderId = Guid.NewGuid();
        private static readonly Guid RecipientId = Guid.NewGuid();
        private static readonly Guid PatientDataIdValue = Guid.NewGuid();

        public CreateDataShareCommandHandlerTests()
        {
            _dataShareRepositoryMock = new Mock<IDataShareRepository>();
            _researcherRepositoryMock = new Mock<IResearcherRepository>();
            _patientDataRepositoryMock = new Mock<IPatientDataRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _handler = new CreateDataShareCommandHandler(
                _dataShareRepositoryMock.Object,
                _researcherRepositoryMock.Object,
                _patientDataRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        private void SetupValidEntities()
        {
            var keys = PublicKeySet.Create("k1", "k2", "k3", "k4", 1);

            _researcherRepositoryMock
                .Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Researcher.Create("Sender", "sender@test.com", "MIT", keys));

            _researcherRepositoryMock
                .Setup(r => r.GetByIdAsync(RecipientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Researcher.Create("Recipient", "recipient@test.com", "Harvard", keys));

            var patientId = PatientIdentifier.Generate();
            _patientDataRepositoryMock
                .Setup(r => r.GetByIdAsync(PatientDataIdValue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Domain.Entities.PatientData.Create(patientId));
        }

        private static CreateDataShareCommand CreateValidCommand() => new()
        {
            SenderResearcherId = SenderId,
            RecipientResearcherId = RecipientId,
            PatientDataId = PatientDataIdValue,
            EncryptedPayload = "encryptedPayload",
            EncapsulatedKey = "encapsulatedKey",
            Signature = "signature",
            SenderKeyVersion = 1,
            RecipientKeyVersion = 1
        };

        [Fact]
        public async Task HandleAsync_ValidCommand_ReturnsSuccessWithGuid()
        {
            SetupValidEntities();

            Result<Guid> result = await _handler.HandleAsync(CreateValidCommand(), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
            _dataShareRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<DataShare>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_SenderNotFound_ReturnsNotFound()
        {
            _researcherRepositoryMock
                .Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            Result<Guid> result = await _handler.HandleAsync(CreateValidCommand(), CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
            Assert.Contains("Sender", result.Error!);
        }

        [Fact]
        public async Task HandleAsync_RecipientNotFound_ReturnsNotFound()
        {
            var keys = PublicKeySet.Create("k1", "k2", "k3", "k4", 1);

            _researcherRepositoryMock
                .Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Researcher.Create("Sender", "sender@test.com", "MIT", keys));

            _researcherRepositoryMock
                .Setup(r => r.GetByIdAsync(RecipientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            Result<Guid> result = await _handler.HandleAsync(CreateValidCommand(), CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
            Assert.Contains("Recipient", result.Error!);
        }

        [Fact]
        public async Task HandleAsync_PatientDataNotFound_ReturnsNotFound()
        {
            var keys = PublicKeySet.Create("k1", "k2", "k3", "k4", 1);

            _researcherRepositoryMock
                .Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Researcher.Create("Sender", "sender@test.com", "MIT", keys));

            _researcherRepositoryMock
                .Setup(r => r.GetByIdAsync(RecipientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Researcher.Create("Recipient", "recipient@test.com", "Harvard", keys));

            _patientDataRepositoryMock
                .Setup(r => r.GetByIdAsync(PatientDataIdValue, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.PatientData?)null);

            Result<Guid> result = await _handler.HandleAsync(CreateValidCommand(), CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
            Assert.Contains("Patient data", result.Error!);
        }
    }
}
