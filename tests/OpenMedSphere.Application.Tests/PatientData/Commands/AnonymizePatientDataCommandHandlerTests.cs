using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.PatientData.Commands.AnonymizePatientData;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;
using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Application.Tests.PatientData.Commands
{
    public sealed class AnonymizePatientDataCommandHandlerTests
    {
        private readonly Mock<IPatientDataRepository> _patientDataRepositoryMock;
        private readonly Mock<IAnonymizationPolicyRepository> _policyRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AnonymizePatientDataCommandHandler _handler;

        public AnonymizePatientDataCommandHandlerTests()
        {
            _patientDataRepositoryMock = new Mock<IPatientDataRepository>();
            _policyRepositoryMock = new Mock<IAnonymizationPolicyRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _handler = new AnonymizePatientDataCommandHandler(
                _patientDataRepositoryMock.Object,
                _policyRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ValidCommand_ReturnsSuccess()
        {
            Guid patientDataId = Guid.NewGuid();
            Guid policyId = Guid.NewGuid();

            Domain.Entities.PatientData patientData =
                Domain.Entities.PatientData.Create(PatientIdentifier.Generate());

            AnonymizationPolicy policy = AnonymizationPolicy.Create("Test Policy", AnonymizationLevel.Standard);

            _patientDataRepositoryMock
                .Setup(r => r.GetByIdAsync(patientDataId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientData);

            _policyRepositoryMock
                .Setup(r => r.GetByIdAsync(policyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(policy);

            AnonymizePatientDataCommand command = new()
            {
                PatientDataId = patientDataId,
                PolicyId = policyId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _patientDataRepositoryMock.Verify(
                r => r.Update(It.IsAny<Domain.Entities.PatientData>()),
                Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_PatientDataNotFound_ReturnsFailure()
        {
            Guid patientDataId = Guid.NewGuid();
            Guid policyId = Guid.NewGuid();

            _patientDataRepositoryMock
                .Setup(r => r.GetByIdAsync(patientDataId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.PatientData?)null);

            AnonymizePatientDataCommand command = new()
            {
                PatientDataId = patientDataId,
                PolicyId = policyId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Contains("not found", result.Error!);
        }

        [Fact]
        public async Task HandleAsync_AlreadyAnonymized_ReturnsFailure()
        {
            Guid patientDataId = Guid.NewGuid();
            Guid policyId = Guid.NewGuid();

            Domain.Entities.PatientData patientData =
                Domain.Entities.PatientData.Create(PatientIdentifier.Generate());
            patientData.MarkAsAnonymized(Guid.NewGuid());

            _patientDataRepositoryMock
                .Setup(r => r.GetByIdAsync(patientDataId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientData);

            AnonymizePatientDataCommand command = new()
            {
                PatientDataId = patientDataId,
                PolicyId = policyId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Contains("already anonymized", result.Error!);
        }

        [Fact]
        public async Task HandleAsync_PolicyNotFound_ReturnsFailure()
        {
            Guid patientDataId = Guid.NewGuid();
            Guid policyId = Guid.NewGuid();

            Domain.Entities.PatientData patientData =
                Domain.Entities.PatientData.Create(PatientIdentifier.Generate());

            _patientDataRepositoryMock
                .Setup(r => r.GetByIdAsync(patientDataId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientData);

            _policyRepositoryMock
                .Setup(r => r.GetByIdAsync(policyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AnonymizationPolicy?)null);

            AnonymizePatientDataCommand command = new()
            {
                PatientDataId = patientDataId,
                PolicyId = policyId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Contains("not found", result.Error!);
        }

        [Fact]
        public async Task HandleAsync_InactivePolicy_ReturnsFailure()
        {
            Guid patientDataId = Guid.NewGuid();
            Guid policyId = Guid.NewGuid();

            Domain.Entities.PatientData patientData =
                Domain.Entities.PatientData.Create(PatientIdentifier.Generate());

            AnonymizationPolicy policy = AnonymizationPolicy.Create("Test Policy", AnonymizationLevel.Standard);
            policy.Deactivate();

            _patientDataRepositoryMock
                .Setup(r => r.GetByIdAsync(patientDataId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientData);

            _policyRepositoryMock
                .Setup(r => r.GetByIdAsync(policyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(policy);

            AnonymizePatientDataCommand command = new()
            {
                PatientDataId = patientDataId,
                PolicyId = policyId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Contains("inactive", result.Error!);
        }
    }
}
