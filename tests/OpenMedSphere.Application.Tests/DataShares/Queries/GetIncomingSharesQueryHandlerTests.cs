using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.DataShares.Queries;
using OpenMedSphere.Application.DataShares.Queries.GetIncomingShares;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Enums;
using Xunit;

namespace OpenMedSphere.Application.Tests.DataShares.Queries
{
    public sealed class GetIncomingSharesQueryHandlerTests
    {
        private readonly Mock<IDataShareRepository> _repositoryMock;
        private readonly GetIncomingSharesQueryHandler _handler;

        private static readonly Guid ResearcherId = Guid.NewGuid();

        public GetIncomingSharesQueryHandlerTests()
        {
            _repositoryMock = new Mock<IDataShareRepository>();
            _handler = new GetIncomingSharesQueryHandler(_repositoryMock.Object);
        }

        private static DataShareSummaryResponse CreateSummary(
            DataShareStatus status = DataShareStatus.Pending,
            DateTime? expiresAtUtc = null) => new()
        {
            Id = Guid.NewGuid(),
            SenderResearcherId = Guid.NewGuid(),
            RecipientResearcherId = ResearcherId,
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
                .Setup(r => r.GetIncomingSharesAsync(ResearcherId, 0, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(summaries);

            GetIncomingSharesQuery query = new() { ResearcherId = ResearcherId };

            Result<IReadOnlyList<DataShareSummaryResponse>> result =
                await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
        }

        [Fact]
        public async Task HandleAsync_WithNoShares_ReturnsEmptyList()
        {
            _repositoryMock
                .Setup(r => r.GetIncomingSharesAsync(ResearcherId, 0, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DataShareSummaryResponse>());

            GetIncomingSharesQuery query = new() { ResearcherId = ResearcherId };

            Result<IReadOnlyList<DataShareSummaryResponse>> result =
                await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task HandleAsync_CalculatesCorrectSkipForPage2()
        {
            _repositoryMock
                .Setup(r => r.GetIncomingSharesAsync(ResearcherId, 20, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DataShareSummaryResponse>());

            GetIncomingSharesQuery query = new()
            {
                ResearcherId = ResearcherId,
                Page = 2,
                PageSize = 20
            };

            await _handler.HandleAsync(query, CancellationToken.None);

            _repositoryMock.Verify(
                r => r.GetIncomingSharesAsync(ResearcherId, 20, 20, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_RespectsCustomPageSize()
        {
            _repositoryMock
                .Setup(r => r.GetIncomingSharesAsync(ResearcherId, 0, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DataShareSummaryResponse>());

            GetIncomingSharesQuery query = new()
            {
                ResearcherId = ResearcherId,
                PageSize = 10
            };

            await _handler.HandleAsync(query, CancellationToken.None);

            _repositoryMock.Verify(
                r => r.GetIncomingSharesAsync(ResearcherId, 0, 10, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
