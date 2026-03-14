using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.DataShares.Commands.RevokeDataShare;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;
using Xunit;

namespace OpenMedSphere.Application.Tests.DataShares.Commands
{
    public sealed class RevokeDataShareCommandHandlerTests
    {
        private readonly Mock<IDataShareRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RevokeDataShareCommandHandler _handler;

        private static readonly Guid SenderId = Guid.NewGuid();
        private static readonly Guid RecipientId = Guid.NewGuid();
        private static readonly Guid PatientDataId = Guid.NewGuid();

        public RevokeDataShareCommandHandlerTests()
        {
            _repositoryMock = new Mock<IDataShareRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _handler = new RevokeDataShareCommandHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        private static DataShare CreatePendingDataShare()
        {
            return DataShare.Create(
                SenderId, RecipientId, PatientDataId,
                "payload", "key", "sig", 1, 1);
        }

        [Fact]
        public async Task HandleAsync_PendingShare_ReturnsSuccess()
        {
            DataShare dataShare = CreatePendingDataShare();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            RevokeDataShareCommand command = new()
            {
                DataShareId = dataShare.Id,
                ResearcherId = SenderId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(DataShareStatus.Revoked, dataShare.Status);
            _repositoryMock.Verify(r => r.Update(dataShare), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_AcceptedShare_ReturnsSuccess()
        {
            DataShare dataShare = CreatePendingDataShare();
            dataShare.Accept();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            RevokeDataShareCommand command = new()
            {
                DataShareId = dataShare.Id,
                ResearcherId = SenderId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(DataShareStatus.Revoked, dataShare.Status);
        }

        [Fact]
        public async Task HandleAsync_DataShareNotFound_ReturnsNotFound()
        {
            Guid dataShareId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShareId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DataShare?)null);

            RevokeDataShareCommand command = new()
            {
                DataShareId = dataShareId,
                ResearcherId = SenderId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
        }

        [Fact]
        public async Task HandleAsync_NotSender_ReturnsInvalidOperation()
        {
            DataShare dataShare = CreatePendingDataShare();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            RevokeDataShareCommand command = new()
            {
                DataShareId = dataShare.Id,
                ResearcherId = RecipientId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.InvalidOperation, result.ErrorCode);
            Assert.Contains("sender", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task HandleAsync_AlreadyRevoked_ReturnsInvalidOperation()
        {
            DataShare dataShare = CreatePendingDataShare();
            dataShare.Revoke();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            RevokeDataShareCommand command = new()
            {
                DataShareId = dataShare.Id,
                ResearcherId = SenderId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.InvalidOperation, result.ErrorCode);
        }
    }
}
