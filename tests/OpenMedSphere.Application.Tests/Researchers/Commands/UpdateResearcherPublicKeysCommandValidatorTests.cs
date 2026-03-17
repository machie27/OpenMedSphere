using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Researchers.Commands.UpdateResearcherPublicKeys;
using Xunit;

namespace OpenMedSphere.Application.Tests.Researchers.Commands
{
    public sealed class UpdateResearcherPublicKeysCommandValidatorTests
    {
        private readonly UpdateResearcherPublicKeysCommandValidator _validator = new();

        private static UpdateResearcherPublicKeysCommand CreateValidCommand() => new()
        {
            ResearcherId = Guid.NewGuid(),
            MlKemPublicKey = Convert.ToBase64String(new byte[] { 1, 2, 3 }),
            MlDsaPublicKey = Convert.ToBase64String(new byte[] { 4, 5, 6 }),
            X25519PublicKey = Convert.ToBase64String(new byte[] { 7, 8, 9 }),
            EcdsaPublicKey = Convert.ToBase64String(new byte[] { 10, 11, 12 }),
            KeyVersion = 2
        };

        [Fact]
        public async Task ValidateAsync_ValidCommand_ReturnsSuccess()
        {
            var result = await _validator.ValidateAsync(CreateValidCommand(), TestContext.Current.CancellationToken);

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_EmptyResearcherId_ReturnsError()
        {
            var command = CreateValidCommand() with { ResearcherId = Guid.Empty };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.ResearcherId));
        }

        [Fact]
        public async Task ValidateAsync_ZeroKeyVersion_ReturnsError()
        {
            var command = CreateValidCommand() with { KeyVersion = 0 };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.KeyVersion));
        }

        [Fact]
        public async Task ValidateAsync_NullMlKemPublicKey_ReturnsError()
        {
            var command = CreateValidCommand() with { MlKemPublicKey = null! };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.MlKemPublicKey));
        }

        [Fact]
        public async Task ValidateAsync_InvalidBase64MlDsaPublicKey_ReturnsError()
        {
            var command = CreateValidCommand() with { MlDsaPublicKey = "not-base64!!!" };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.MlDsaPublicKey));
        }

        [Fact]
        public async Task ValidateAsync_NullX25519PublicKey_ReturnsError()
        {
            var command = CreateValidCommand() with { X25519PublicKey = null! };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.X25519PublicKey));
        }

        [Fact]
        public async Task ValidateAsync_InvalidBase64X25519PublicKey_ReturnsError()
        {
            var command = CreateValidCommand() with { X25519PublicKey = "not-base64!!!" };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.X25519PublicKey));
        }

        [Fact]
        public async Task ValidateAsync_NullEcdsaPublicKey_ReturnsError()
        {
            var command = CreateValidCommand() with { EcdsaPublicKey = null! };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.EcdsaPublicKey));
        }

        [Fact]
        public async Task ValidateAsync_InvalidBase64EcdsaPublicKey_ReturnsError()
        {
            var command = CreateValidCommand() with { EcdsaPublicKey = "not-base64!!!" };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.EcdsaPublicKey));
        }

        [Fact]
        public async Task ValidateAsync_NullMlDsaPublicKey_ReturnsError()
        {
            var command = CreateValidCommand() with { MlDsaPublicKey = null! };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.MlDsaPublicKey));
        }
    }
}
