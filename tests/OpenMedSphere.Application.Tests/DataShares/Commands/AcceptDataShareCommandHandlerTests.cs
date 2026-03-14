using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.DataShares.Commands.AcceptDataShare;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;
using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Application.Tests.DataShares.Commands
{
    public sealed class AcceptDataShareCommandHandlerTests
    {
        private readonly Mock<IDataShareRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AcceptDataShareCommandHandler _handler;

        private static readonly Guid SenderId = Guid.NewGuid();
        private static readonly Guid RecipientId = Guid.NewGuid();
        private static readonly Guid PatientDataId = Guid.NewGuid();

        public AcceptDataShareCommandHandlerTests()
        {
            _repositoryMock = new Mock<IDataShareRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _handler = new AcceptDataShareCommandHandler(
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
        public async Task HandleAsync_ValidCommand_ReturnsSuccess()
        {
            DataShare dataShare = CreatePendingDataShare();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            AcceptDataShareCommand command = new()
            {
                DataShareId = dataShare.Id,
                ResearcherId = RecipientId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(DataShareStatus.Accepted, dataShare.Status);
            Assert.NotNull(dataShare.AccessedAtUtc);
            _repositoryMock.Verify(r => r.Update(dataShare), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_DataShareNotFound_ReturnsNotFound()
        {
            Guid dataShareId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShareId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DataShare?)null);

            AcceptDataShareCommand command = new()
            {
                DataShareId = dataShareId,
                ResearcherId = RecipientId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
        }

        [Fact]
        public async Task HandleAsync_NotRecipient_ReturnsInvalidOperation()
        {
            DataShare dataShare = CreatePendingDataShare();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            AcceptDataShareCommand command = new()
            {
                DataShareId = dataShare.Id,
                ResearcherId = SenderId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.InvalidOperation, result.ErrorCode);
            Assert.Contains("recipient", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task HandleAsync_AlreadyAccepted_ReturnsInvalidOperation()
        {
            DataShare dataShare = CreatePendingDataShare();
            dataShare.Accept();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            AcceptDataShareCommand command = new()
            {
                DataShareId = dataShare.Id,
                ResearcherId = RecipientId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.InvalidOperation, result.ErrorCode);
        }

        [Fact]
        public async Task HandleAsync_AlreadyRevoked_ReturnsInvalidOperation()
        {
            DataShare dataShare = CreatePendingDataShare();
            dataShare.Revoke();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            AcceptDataShareCommand command = new()
            {
                DataShareId = dataShare.Id,
                ResearcherId = RecipientId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.InvalidOperation, result.ErrorCode);
        }

        [Fact]
        public async Task HandleAsync_ExpiredShare_ReturnsInvalidOperation()
        {
            DataShare dataShare = DataShare.Create(
                SenderId, RecipientId, PatientDataId,
                "payload", "key", "sig", 1, 1,
                DateTime.UtcNow.AddMilliseconds(1));

            Thread.Sleep(10);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            AcceptDataShareCommand command = new()
            {
                DataShareId = dataShare.Id,
                ResearcherId = RecipientId
            };

            Result result = await _handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.InvalidOperation, result.ErrorCode);
            Assert.Contains("expired", result.Error!, StringComparison.OrdinalIgnoreCase);
        }
    }
}
