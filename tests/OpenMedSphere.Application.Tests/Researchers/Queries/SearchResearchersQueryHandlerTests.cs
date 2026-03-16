using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Researchers.Queries;
using OpenMedSphere.Application.Researchers.Queries.SearchResearchers;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.ValueObjects;
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

        private static Researcher CreateResearcher(string name, string institution) =>
            Researcher.Create(name, $"{name.Replace(" ", "").Replace(".", "").ToLower()}@test.com", institution,
                PublicKeySet.Create("k1", "k2", "k3", "k4", 1));

        [Fact]
        public async Task HandleAsync_WithResults_ReturnsMappedSummaries()
        {
            var researchers = new List<Researcher>
            {
                CreateResearcher("Dr. Smith", "MIT"),
                CreateResearcher("Dr. Jones", "Harvard")
            };

            _repositoryMock
                .Setup(r => r.SearchAsync("smith", 0, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(researchers);

            SearchResearchersQuery query = new() { Query = "smith" };

            Result<IReadOnlyList<ResearcherSummaryResponse>> result =
                await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
            Assert.Equal("Dr. Smith", result.Value[0].Name);
            Assert.Equal("MIT", result.Value[0].Institution);
        }

        [Fact]
        public async Task HandleAsync_MapsOnlyIdNameInstitution()
        {
            var researcher = CreateResearcher("Dr. Smith", "MIT");
            _repositoryMock
                .Setup(r => r.SearchAsync("smith", 0, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Researcher> { researcher });

            SearchResearchersQuery query = new() { Query = "smith" };

            Result<IReadOnlyList<ResearcherSummaryResponse>> result =
                await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            var summary = result.Value[0];
            Assert.Equal(researcher.Id, summary.Id);
            Assert.Equal(researcher.Name, summary.Name);
            Assert.Equal(researcher.Institution, summary.Institution);
        }

        [Fact]
        public async Task HandleAsync_WithNoResults_ReturnsEmptyList()
        {
            _repositoryMock
                .Setup(r => r.SearchAsync("nonexistent", 0, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Researcher>());

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
                .ReturnsAsync(new List<Researcher>());

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
                .ReturnsAsync(new List<Researcher>());

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
