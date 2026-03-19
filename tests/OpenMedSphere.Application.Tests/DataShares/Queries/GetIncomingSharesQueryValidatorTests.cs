using OpenMedSphere.Application.DataShares.Queries.GetIncomingShares;
using OpenMedSphere.Application.Messaging;
using Xunit;

namespace OpenMedSphere.Application.Tests.DataShares.Queries
{
    public sealed class GetIncomingSharesQueryValidatorTests
    {
        private readonly GetIncomingSharesQueryValidator _validator = new();

        private static GetIncomingSharesQuery CreateValidQuery() => new()
        {
            ResearcherId = Guid.CreateVersion7(),
            Page = 1,
            PageSize = 20
        };

        [Fact]
        public async Task ValidateAsync_ValidQuery_ReturnsSuccess()
        {
            var result = await _validator.ValidateAsync(CreateValidQuery(), TestContext.Current.CancellationToken);

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_EmptyResearcherId_ReturnsError()
        {
            var query = CreateValidQuery() with { ResearcherId = Guid.Empty };

            var result = await _validator.ValidateAsync(query, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(query.ResearcherId));
        }

        [Fact]
        public async Task ValidateAsync_PageLessThanOne_ReturnsError()
        {
            var query = CreateValidQuery() with { Page = 0 };

            var result = await _validator.ValidateAsync(query, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(query.Page));
        }

        [Fact]
        public async Task ValidateAsync_NegativePage_ReturnsError()
        {
            var query = CreateValidQuery() with { Page = -1 };

            var result = await _validator.ValidateAsync(query, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(query.Page));
        }

        [Fact]
        public async Task ValidateAsync_PageSizeExceedsMax_ReturnsError()
        {
            var query = CreateValidQuery() with { PageSize = 101 };

            var result = await _validator.ValidateAsync(query, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(query.PageSize));
        }

        [Fact]
        public async Task ValidateAsync_PageSizeZero_ReturnsError()
        {
            var query = CreateValidQuery() with { PageSize = 0 };

            var result = await _validator.ValidateAsync(query, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(query.PageSize));
        }

        [Fact]
        public async Task ValidateAsync_PageSizeAtMax_ReturnsSuccess()
        {
            var query = CreateValidQuery() with { PageSize = 100 };

            var result = await _validator.ValidateAsync(query, TestContext.Current.CancellationToken);

            Assert.True(result.IsValid);
        }
    }
}
