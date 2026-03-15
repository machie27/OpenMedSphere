using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Researchers.Commands.RegisterResearcher;
using Xunit;

namespace OpenMedSphere.Application.Tests.Researchers.Commands
{
    public sealed class RegisterResearcherCommandValidatorTests
    {
        private readonly RegisterResearcherCommandValidator _validator = new();

        private static RegisterResearcherCommand CreateValidCommand() => new()
        {
            Name = "Dr. Smith",
            Email = "smith@university.edu",
            Institution = "MIT",
            MlKemPublicKey = Convert.ToBase64String(new byte[] { 1, 2, 3 }),
            MlDsaPublicKey = Convert.ToBase64String(new byte[] { 4, 5, 6 }),
            X25519PublicKey = Convert.ToBase64String(new byte[] { 7, 8, 9 }),
            EcdsaPublicKey = Convert.ToBase64String(new byte[] { 10, 11, 12 })
        };

        [Fact]
        public async Task ValidateAsync_ValidCommand_ReturnsSuccess()
        {
            var result = await _validator.ValidateAsync(CreateValidCommand(), TestContext.Current.CancellationToken);

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_NullName_ReturnsError()
        {
            var command = CreateValidCommand() with { Name = null! };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Name));
        }

        [Fact]
        public async Task ValidateAsync_NullEmail_ReturnsError()
        {
            var command = CreateValidCommand() with { Email = null! };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Email));
        }

        [Fact]
        public async Task ValidateAsync_InvalidEmail_ReturnsError()
        {
            var command = CreateValidCommand() with { Email = "not-an-email" };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Email));
        }

        [Fact]
        public async Task ValidateAsync_NullInstitution_ReturnsError()
        {
            var command = CreateValidCommand() with { Institution = null! };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Institution));
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
        public async Task ValidateAsync_InvalidBase64MlKemPublicKey_ReturnsError()
        {
            var command = CreateValidCommand() with { MlKemPublicKey = "not-base64!!!" };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.MlKemPublicKey));
        }

        [Fact]
        public async Task ValidateAsync_OverLengthName_ReturnsError()
        {
            var command = CreateValidCommand() with { Name = new string('a', 201) };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Name));
        }
    }
}
