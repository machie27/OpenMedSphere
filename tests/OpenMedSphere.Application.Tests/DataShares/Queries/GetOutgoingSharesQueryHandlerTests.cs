using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.DataShares.Queries;
using OpenMedSphere.Application.DataShares.Queries.GetOutgoingShares;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Enums;
using Xunit;

namespace OpenMedSphere.Application.Tests.DataShares.Queries
{
    public sealed class GetOutgoingSharesQueryHandlerTests
    {
        private readonly Mock<IDataShareRepository> _repositoryMock;
        private readonly GetOutgoingSharesQueryHandler _handler;

        private static readonly Guid ResearcherId = Guid.NewGuid();

        public GetOutgoingSharesQueryHandlerTests()
        {
            _repositoryMock = new Mock<IDataShareRepository>();
            _handler = new GetOutgoingSharesQueryHandler(_repositoryMock.Object);
        }

        private static DataShareSummaryResponse CreateSummary(
            DataShareStatus status = DataShareStatus.Pending,
            DateTime? expiresAtUtc = null) => new()
        {
            Id = Guid.NewGuid(),
            SenderResearcherId = ResearcherId,
            RecipientResearcherId = Guid.NewGuid(),
            PatientDataId = Guid.NewGuid(),
            Status = status,
            SharedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = expiresAtUtc
        };

        [Fact]
        public async Task HandleAsync_WithShares_ReturnsSuccessWithSummaries()
        {
            var summaries = new List<DataShareSummaryResponse>
            {
                CreateSummary(),
                CreateSummary(DataShareStatus.Accepted)
            };

            _repositoryMock
                .Setup(r => r.GetOutgoingSharesAsync(ResearcherId, 0, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(summaries);

            GetOutgoingSharesQuery query = new() { ResearcherId = ResearcherId };

            Result<IReadOnlyList<DataShareSummaryResponse>> result =
                await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
        }

        [Fact]
        public async Task HandleAsync_WithNoShares_ReturnsEmptyList()
        {
            _repositoryMock
                .Setup(r => r.GetOutgoingSharesAsync(ResearcherId, 0, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DataShareSummaryResponse>());

            GetOutgoingSharesQuery query = new() { ResearcherId = ResearcherId };

            Result<IReadOnlyList<DataShareSummaryResponse>> result =
                await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task HandleAsync_CalculatesCorrectSkipForPage3()
        {
            _repositoryMock
                .Setup(r => r.GetOutgoingSharesAsync(ResearcherId, 50, 25, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DataShareSummaryResponse>());

            GetOutgoingSharesQuery query = new()
            {
                ResearcherId = ResearcherId,
                Page = 3,
                PageSize = 25
            };

            await _handler.HandleAsync(query, CancellationToken.None);

            _repositoryMock.Verify(
                r => r.GetOutgoingSharesAsync(ResearcherId, 50, 25, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_RespectsCustomPageSize()
        {
            _repositoryMock
                .Setup(r => r.GetOutgoingSharesAsync(ResearcherId, 0, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DataShareSummaryResponse>());

            GetOutgoingSharesQuery query = new()
            {
                ResearcherId = ResearcherId,
                PageSize = 5
            };

            await _handler.HandleAsync(query, CancellationToken.None);

            _repositoryMock.Verify(
                r => r.GetOutgoingSharesAsync(ResearcherId, 0, 5, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
