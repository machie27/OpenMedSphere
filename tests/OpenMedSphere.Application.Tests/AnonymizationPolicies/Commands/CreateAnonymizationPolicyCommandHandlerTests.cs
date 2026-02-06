using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.AnonymizationPolicies.Commands.CreatePolicy;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;
using Xunit;

namespace OpenMedSphere.Application.Tests.AnonymizationPolicies.Commands
{
    public sealed class CreateAnonymizationPolicyCommandHandlerTests
    {
        private readonly Mock<IAnonymizationPolicyRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateAnonymizationPolicyCommandHandler _handler;

        public CreateAnonymizationPolicyCommandHandlerTests()
        {
            _repositoryMock = new Mock<IAnonymizationPolicyRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _handler = new CreateAnonymizationPolicyCommandHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ValidCommand_ReturnsSuccessWithGuid()
        {
            CreateAnonymizationPolicyCommand command = new()
            {
                Name = "HIPAA Standard Policy",
                Description = "Standard HIPAA anonymization policy",
                Level = AnonymizationLevel.Standard
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        }

        [Fact]
        public async Task HandleAsync_ValidCommand_CallsAddAndSaveChanges()
        {
            CreateAnonymizationPolicyCommand command = new()
            {
                Name = "Advanced Research Policy",
                Description = "Advanced anonymization for sensitive research",
                Level = AnonymizationLevel.Advanced
            };

            Result<Guid> result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(
                r => r.AddAsync(
                    It.Is<AnonymizationPolicy>(p =>
                        p.Name == "Advanced Research Policy" &&
                        p.Level == AnonymizationLevel.Advanced),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
