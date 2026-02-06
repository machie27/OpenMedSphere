using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Abstractions.MedicalTerminology;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.PatientData.Commands.CreatePatientData;
using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Application.Tests.PatientData.Commands
{
    public sealed class CreatePatientDataCommandHandlerTests
    {
        private readonly Mock<IPatientDataRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMedicalTerminologyService> _terminologyServiceMock;
        private readonly CreatePatientDataCommandHandler _handler;

        public CreatePatientDataCommandHandlerTests()
        {
            _repositoryMock = new Mock<IPatientDataRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _terminologyServiceMock = new Mock<IMedicalTerminologyService>();

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _handler = new CreatePatientDataCommandHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object,
                _terminologyServiceMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ValidCommand_ReturnsSuccessWithGuid()
        {
            CreatePatientDataCommand command = new()
            {
                PrimaryDiagnosis = "Hypertension"
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Domain.Entities.PatientData>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithIcdCode_SetsCodeOnPatientData()
        {
            MedicalCode medicalCode = MedicalCode.Create("BA00", "Essential hypertension", "ICD-11");

            _terminologyServiceMock
                .Setup(s => s.GetByCodeAsync("BA00", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(medicalCode);

            CreatePatientDataCommand command = new()
            {
                PrimaryDiagnosisIcdCode = "BA00"
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _terminologyServiceMock.Verify(
                s => s.GetByCodeAsync("BA00", null, It.IsAny<CancellationToken>()),
                Times.Once);
            _repositoryMock.Verify(
                r => r.AddAsync(
                    It.Is<Domain.Entities.PatientData>(p => p.PrimaryDiagnosisCode == medicalCode),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithInvalidIcdCode_ReturnsFailure()
        {
            _terminologyServiceMock
                .Setup(s => s.GetByCodeAsync("INVALID", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((MedicalCode?)null);

            CreatePatientDataCommand command = new()
            {
                PrimaryDiagnosisIcdCode = "INVALID"
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Contains("INVALID", result.Error!);
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Domain.Entities.PatientData>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithDemographics_SetsDemographicsOnPatientData()
        {
            CreatePatientDataCommand command = new()
            {
                YearOfBirth = 1985,
                Gender = "Female",
                Region = "Northeast"
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(
                r => r.AddAsync(
                    It.Is<Domain.Entities.PatientData>(p =>
                        p.YearOfBirth == 1985 &&
                        p.Gender == "Female" &&
                        p.Region == "Northeast"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithSecondaryDiagnoses_AddsThemToPatientData()
        {
            List<string> secondaryDiagnoses = new() { "Diabetes", "Asthma" };

            CreatePatientDataCommand command = new()
            {
                PrimaryDiagnosis = "Hypertension",
                SecondaryDiagnoses = secondaryDiagnoses
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(
                r => r.AddAsync(
                    It.Is<Domain.Entities.PatientData>(p =>
                        p.SecondaryDiagnoses.Count == 2 &&
                        p.SecondaryDiagnoses.Contains("Diabetes") &&
                        p.SecondaryDiagnoses.Contains("Asthma")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
