using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Researchers.Queries.SearchResearchers;
using Xunit;

namespace OpenMedSphere.Application.Tests.Researchers.Queries
{
    public sealed class SearchResearchersQueryValidatorTests
    {
        private readonly SearchResearchersQueryValidator _validator = new();

        private static SearchResearchersQuery CreateValidQuery() => new()
        {
            Query = "test",
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
        public async Task ValidateAsync_EmptyQuery_ReturnsError()
        {
            var query = CreateValidQuery() with { Query = "" };

            var result = await _validator.ValidateAsync(query, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(query.Query));
        }

        [Fact]
        public async Task ValidateAsync_WhitespaceQuery_ReturnsError()
        {
            var query = CreateValidQuery() with { Query = "   " };

            var result = await _validator.ValidateAsync(query, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(query.Query));
        }

        [Fact]
        public async Task ValidateAsync_OverLengthQuery_ReturnsError()
        {
            var query = CreateValidQuery() with { Query = new string('a', 201) };

            var result = await _validator.ValidateAsync(query, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(query.Query));
        }

        [Fact]
        public async Task ValidateAsync_QueryAtMaxLength_ReturnsSuccess()
        {
            var query = CreateValidQuery() with { Query = new string('a', 200) };

            var result = await _validator.ValidateAsync(query, TestContext.Current.CancellationToken);

            Assert.True(result.IsValid);
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
