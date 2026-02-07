using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.ResearchStudies.Commands.CreateResearchStudy;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;
using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Application.Tests.ResearchStudies.Commands
{
    public sealed class CreateResearchStudyCommandHandlerTests
    {
        private readonly Mock<IResearchStudyRepository> _repositoryMock;
        private readonly Mock<IAnonymizationPolicyRepository> _policyRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateResearchStudyCommandHandler _handler;

        public CreateResearchStudyCommandHandlerTests()
        {
            _repositoryMock = new Mock<IResearchStudyRepository>();
            _policyRepositoryMock = new Mock<IAnonymizationPolicyRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _handler = new CreateResearchStudyCommandHandler(
                _repositoryMock.Object,
                _policyRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ValidCommand_ReturnsSuccessWithGuid()
        {
            Guid policyId = Guid.NewGuid();

            AnonymizationPolicy policy = AnonymizationPolicy.Create("Test Policy", AnonymizationLevel.Standard);

            _policyRepositoryMock
                .Setup(r => r.GetByIdAsync(policyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(policy);

            _repositoryMock
                .Setup(r => r.GetByCodeAsync("STUDY-001", It.IsAny<CancellationToken>()))
                .ReturnsAsync((ResearchStudy?)null);

            CreateResearchStudyCommand command = new()
            {
                StudyCode = "STUDY-001",
                Title = "Test Research Study",
                PrincipalInvestigator = "Dr. Smith",
                Institution = "Test University",
                StudyPeriodStart = DateTime.UtcNow,
                StudyPeriodEnd = DateTime.UtcNow.AddYears(1),
                AnonymizationPolicyId = policyId
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<ResearchStudy>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_PolicyNotFound_ReturnsFailure()
        {
            Guid policyId = Guid.NewGuid();

            _policyRepositoryMock
                .Setup(r => r.GetByIdAsync(policyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AnonymizationPolicy?)null);

            CreateResearchStudyCommand command = new()
            {
                StudyCode = "STUDY-002",
                Title = "Test Research Study",
                PrincipalInvestigator = "Dr. Smith",
                Institution = "Test University",
                StudyPeriodStart = DateTime.UtcNow,
                StudyPeriodEnd = DateTime.UtcNow.AddYears(1),
                AnonymizationPolicyId = policyId
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Contains("not found", result.Error!);
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<ResearchStudy>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_DuplicateStudyCode_ReturnsFailure()
        {
            Guid policyId = Guid.NewGuid();

            AnonymizationPolicy policy = AnonymizationPolicy.Create("Test Policy", AnonymizationLevel.Standard);

            StudyCode studyCode = StudyCode.Create("STUDY-003");
            DateRange studyPeriod = DateRange.Create(DateTime.UtcNow, DateTime.UtcNow.AddYears(1));

            ResearchStudy existingStudy = ResearchStudy.Create(
                studyCode,
                "Existing Study",
                "Dr. Jones",
                "Other University",
                studyPeriod,
                policyId);

            _policyRepositoryMock
                .Setup(r => r.GetByIdAsync(policyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(policy);

            _repositoryMock
                .Setup(r => r.GetByCodeAsync("STUDY-003", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingStudy);

            CreateResearchStudyCommand command = new()
            {
                StudyCode = "STUDY-003",
                Title = "New Study With Same Code",
                PrincipalInvestigator = "Dr. Smith",
                Institution = "Test University",
                StudyPeriodStart = DateTime.UtcNow,
                StudyPeriodEnd = DateTime.UtcNow.AddYears(1),
                AnonymizationPolicyId = policyId
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Contains("already exists", result.Error!);
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<ResearchStudy>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
