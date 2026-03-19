using OpenMedSphere.Application.DataShares.Commands.CreateDataShare;
using OpenMedSphere.Application.Messaging;
using Xunit;

namespace OpenMedSphere.Application.Tests.DataShares.Commands
{
    public sealed class CreateDataShareCommandValidatorTests
    {
        private readonly CreateDataShareCommandValidator _validator = new();

        private static CreateDataShareCommand CreateValidCommand() => new()
        {
            SenderResearcherId = Guid.NewGuid(),
            RecipientResearcherId = Guid.NewGuid(),
            PatientDataId = Guid.NewGuid(),
            EncryptedPayload = Convert.ToBase64String(new byte[] { 1, 2, 3 }),
            EncapsulatedKey = Convert.ToBase64String(new byte[] { 4, 5, 6 }),
            Signature = Convert.ToBase64String(new byte[] { 7, 8, 9 }),
            SenderKeyVersion = 1,
            RecipientKeyVersion = 1,
            ExpiresAtUtc = DateTime.UtcNow.AddHours(1)
        };

        [Fact]
        public async Task ValidateAsync_ValidCommand_ReturnsSuccess()
        {
            var result = await _validator.ValidateAsync(CreateValidCommand(), TestContext.Current.CancellationToken);

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_EmptySenderResearcherId_ReturnsError()
        {
            var command = CreateValidCommand() with { SenderResearcherId = Guid.Empty };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.SenderResearcherId));
        }

        [Fact]
        public async Task ValidateAsync_EmptyRecipientResearcherId_ReturnsError()
        {
            var command = CreateValidCommand() with { RecipientResearcherId = Guid.Empty };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.RecipientResearcherId));
        }

        [Fact]
        public async Task ValidateAsync_SameSenderAndRecipient_ReturnsError()
        {
            var sharedId = Guid.NewGuid();
            var command = CreateValidCommand() with
            {
                SenderResearcherId = sharedId,
                RecipientResearcherId = sharedId
            };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.RecipientResearcherId));
        }

        [Fact]
        public async Task ValidateAsync_EmptyPatientDataId_ReturnsError()
        {
            var command = CreateValidCommand() with { PatientDataId = Guid.Empty };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.PatientDataId));
        }

        [Fact]
        public async Task ValidateAsync_NullEncryptedPayload_ReturnsError()
        {
            var command = CreateValidCommand() with { EncryptedPayload = null! };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.EncryptedPayload));
        }

        [Fact]
        public async Task ValidateAsync_InvalidBase64EncryptedPayload_ReturnsError()
        {
            var command = CreateValidCommand() with { EncryptedPayload = "not-base64!!!" };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.EncryptedPayload));
        }

        [Fact]
        public async Task ValidateAsync_NullEncapsulatedKey_ReturnsError()
        {
            var command = CreateValidCommand() with { EncapsulatedKey = null! };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.EncapsulatedKey));
        }

        [Fact]
        public async Task ValidateAsync_NullSignature_ReturnsError()
        {
            var command = CreateValidCommand() with { Signature = null! };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Signature));
        }

        [Fact]
        public async Task ValidateAsync_ZeroSenderKeyVersion_ReturnsError()
        {
            var command = CreateValidCommand() with { SenderKeyVersion = 0 };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.SenderKeyVersion));
        }

        [Fact]
        public async Task ValidateAsync_ZeroRecipientKeyVersion_ReturnsError()
        {
            var command = CreateValidCommand() with { RecipientKeyVersion = 0 };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.RecipientKeyVersion));
        }

        [Fact]
        public async Task ValidateAsync_PastExpiryDate_ReturnsError()
        {
            var command = CreateValidCommand() with { ExpiresAtUtc = DateTime.UtcNow.AddHours(-1) };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.ExpiresAtUtc));
        }

        [Fact]
        public async Task ValidateAsync_NullExpiryDate_ReturnsSuccess()
        {
            var command = CreateValidCommand() with { ExpiresAtUtc = null };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.True(result.IsValid);
        }
    }
}
