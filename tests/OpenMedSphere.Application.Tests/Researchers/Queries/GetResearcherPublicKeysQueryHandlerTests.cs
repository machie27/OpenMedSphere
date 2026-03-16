using Moq;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Researchers.Queries;
using OpenMedSphere.Application.Researchers.Queries.GetResearcherPublicKeys;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Application.Tests.Researchers.Queries
{
    public sealed class GetResearcherPublicKeysQueryHandlerTests
    {
        private readonly Mock<IResearcherRepository> _repositoryMock;
        private readonly GetResearcherPublicKeysQueryHandler _handler;

        public GetResearcherPublicKeysQueryHandlerTests()
        {
            _repositoryMock = new Mock<IResearcherRepository>();
            _handler = new GetResearcherPublicKeysQueryHandler(_repositoryMock.Object);
        }

        private static Researcher CreateResearcher() =>
            Researcher.Create("Dr. Smith", "smith@test.com", "MIT",
                PublicKeySet.Create("mlkem-key", "mldsa-key", "x25519-key", "ecdsa-key", 1));

        [Fact]
        public async Task HandleAsync_ExistingResearcher_ReturnsMappedPublicKeys()
        {
            var researcher = CreateResearcher();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(researcher.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(researcher);

            GetResearcherPublicKeysQuery query = new() { Id = researcher.Id };

            Result<PublicKeySetResponse> result = await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("mlkem-key", result.Value.MlKemPublicKey);
            Assert.Equal("mldsa-key", result.Value.MlDsaPublicKey);
            Assert.Equal("x25519-key", result.Value.X25519PublicKey);
            Assert.Equal("ecdsa-key", result.Value.EcdsaPublicKey);
            Assert.Equal(1, result.Value.KeyVersion);
        }

        [Fact]
        public async Task HandleAsync_NonExistentResearcher_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Researcher?)null);

            GetResearcherPublicKeysQuery query = new() { Id = id };

            Result<PublicKeySetResponse> result = await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
        }

        [Fact]
        public async Task HandleAsync_InactiveResearcher_StillReturnsKeys()
        {
            var researcher = CreateResearcher();
            researcher.Deactivate();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(researcher.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(researcher);

            GetResearcherPublicKeysQuery query = new() { Id = researcher.Id };

            Result<PublicKeySetResponse> result = await _handler.HandleAsync(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("mlkem-key", result.Value.MlKemPublicKey);
        }
    }
}
