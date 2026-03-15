using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.DataShares.Queries;
using OpenMedSphere.Application.DataShares.Queries.GetDataShareById;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;
using Xunit;

namespace OpenMedSphere.Application.Tests.DataShares.Queries
{
    public sealed class GetDataShareByIdQueryHandlerTests
    {
        private readonly Mock<IDataShareRepository> _repositoryMock;
        private readonly GetDataShareByIdQueryHandler _handler;

        private static readonly Guid SenderId = Guid.NewGuid();
        private static readonly Guid RecipientId = Guid.NewGuid();
        private static readonly Guid PatientDataId = Guid.NewGuid();

        public GetDataShareByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IDataShareRepository>();
            _handler = new GetDataShareByIdQueryHandler(_repositoryMock.Object);
        }

        private static DataShare CreatePendingDataShare()
        {
            return DataShare.Create(
                SenderId, RecipientId, PatientDataId,
                "payload", "key", "sig", 1, 1);
        }

        [Fact]
        public async Task HandleAsync_AsSender_ReturnsSuccess()
        {
            DataShare dataShare = CreatePendingDataShare();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            GetDataShareByIdQuery query = new()
            {
                Id = dataShare.Id,
                ResearcherId = SenderId
            };

            Result<DataShareResponse> result = await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(dataShare.Id, result.Value!.Id);
        }

        [Fact]
        public async Task HandleAsync_AsRecipient_ReturnsSuccess()
        {
            DataShare dataShare = CreatePendingDataShare();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            GetDataShareByIdQuery query = new()
            {
                Id = dataShare.Id,
                ResearcherId = RecipientId
            };

            Result<DataShareResponse> result = await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(dataShare.Id, result.Value!.Id);
        }

        [Fact]
        public async Task HandleAsync_NonParticipant_ReturnsNotFound()
        {
            DataShare dataShare = CreatePendingDataShare();
            Guid unrelatedResearcherId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            GetDataShareByIdQuery query = new()
            {
                Id = dataShare.Id,
                ResearcherId = unrelatedResearcherId
            };

            Result<DataShareResponse> result = await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
        }

        [Fact]
        public async Task HandleAsync_DataShareNotFound_ReturnsNotFound()
        {
            Guid dataShareId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShareId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DataShare?)null);

            GetDataShareByIdQuery query = new()
            {
                Id = dataShareId,
                ResearcherId = SenderId
            };

            Result<DataShareResponse> result = await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
        }

        [Fact]
        public async Task HandleAsync_ReturnsEffectiveStatus()
        {
            DataShare dataShare = CreatePendingDataShare();
            dataShare.Accept();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(dataShare.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataShare);

            GetDataShareByIdQuery query = new()
            {
                Id = dataShare.Id,
                ResearcherId = RecipientId
            };

            Result<DataShareResponse> result = await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(DataShareStatus.Accepted, result.Value!.Status);
        }
    }
}
