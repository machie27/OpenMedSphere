using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Researchers.Queries;
using OpenMedSphere.Application.Researchers.Queries.GetResearcherById;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Application.Tests.Researchers.Queries
{
    public sealed class GetResearcherByIdQueryHandlerTests
    {
        private readonly Mock<IResearcherRepository> _repositoryMock;
        private readonly GetResearcherByIdQueryHandler _handler;

        public GetResearcherByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IResearcherRepository>();
            _handler = new GetResearcherByIdQueryHandler(_repositoryMock.Object);
        }

        private static Researcher CreateResearcher() =>
            Researcher.Create("Dr. Smith", "smith@test.com", "MIT",
                PublicKeySet.Create("k1", "k2", "k3", "k4", 1));

        [Fact]
        public async Task HandleAsync_ExistingResearcher_ReturnsMappedResponse()
        {
            var researcher = CreateResearcher();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(researcher.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(researcher);

            GetResearcherByIdQuery query = new() { Id = researcher.Id };

            Result<ResearcherResponse> result = await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(researcher.Id, result.Value.Id);
            Assert.Equal("Dr. Smith", result.Value.Name);
            Assert.Equal("smith@test.com", result.Value.Email);
            Assert.Equal("MIT", result.Value.Institution);
            Assert.Equal(1, result.Value.KeyVersion);
            Assert.True(result.Value.IsActive);
            Assert.Equal(researcher.CreatedAtUtc, result.Value.CreatedAtUtc);
        }

        [Fact]
        public async Task HandleAsync_NonExistentResearcher_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            GetResearcherByIdQuery query = new() { Id = id };

            Result<ResearcherResponse> result = await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
        }
    }
}
