using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Researchers.Queries;
using OpenMedSphere.Application.Researchers.Queries.SearchResearchers;
using Xunit;

namespace OpenMedSphere.Application.Tests.Researchers.Queries
{
    public sealed class SearchResearchersQueryHandlerTests
    {
        private readonly Mock<IResearcherRepository> _repositoryMock;
        private readonly SearchResearchersQueryHandler _handler;

        public SearchResearchersQueryHandlerTests()
        {
            _repositoryMock = new Mock<IResearcherRepository>();
            _handler = new SearchResearchersQueryHandler(_repositoryMock.Object);
        }

        private static ResearcherSummaryResponse CreateSummary(string name, string institution) =>
            new()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Institution = institution
            };

        [Fact]
        public async Task HandleAsync_WithResults_ReturnsSummaries()
        {
            var summaries = new List<ResearcherSummaryResponse>
            {
                CreateSummary("Dr. Smith", "MIT"),
                CreateSummary("Dr. Jones", "Harvard")
            };

            _repositoryMock
                .Setup(r => r.SearchAsync("smith", 0, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(summaries);

            SearchResearchersQuery query = new() { Query = "smith" };

            Result<IReadOnlyList<ResearcherSummaryResponse>> result =
                await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
            Assert.Equal("Dr. Smith", result.Value[0].Name);
            Assert.Equal("MIT", result.Value[0].Institution);
        }

        [Fact]
        public async Task HandleAsync_WithNoResults_ReturnsEmptyList()
        {
            _repositoryMock
                .Setup(r => r.SearchAsync("nonexistent", 0, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ResearcherSummaryResponse>());

            SearchResearchersQuery query = new() { Query = "nonexistent" };

            Result<IReadOnlyList<ResearcherSummaryResponse>> result =
                await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task HandleAsync_CalculatesCorrectSkipForPage2()
        {
            _repositoryMock
                .Setup(r => r.SearchAsync("test", 20, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ResearcherSummaryResponse>());

            SearchResearchersQuery query = new()
            {
                Query = "test",
                Page = 2,
                PageSize = 20
            };

            await _handler.HandleAsync(query, CancellationToken.None);

            _repositoryMock.Verify(
                r => r.SearchAsync("test", 20, 20, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_RespectsCustomPageSize()
        {
            _repositoryMock
                .Setup(r => r.SearchAsync("test", 0, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ResearcherSummaryResponse>());

            SearchResearchersQuery query = new()
            {
                Query = "test",
                PageSize = 10
            };

            await _handler.HandleAsync(query, CancellationToken.None);

            _repositoryMock.Verify(
                r => r.SearchAsync("test", 0, 10, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
