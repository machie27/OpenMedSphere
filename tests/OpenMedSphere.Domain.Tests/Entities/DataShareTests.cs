using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;
using OpenMedSphere.Domain.Events;
using Xunit;

namespace OpenMedSphere.Domain.Tests.Entities
{
    public sealed class DataShareTests
    {
        private static readonly Guid SenderId = Guid.NewGuid();
        private static readonly Guid RecipientId = Guid.NewGuid();
        private static readonly Guid PatientDataId = Guid.NewGuid();

        private static DataShare CreateTestShare(DateTime? expiresAtUtc = null) =>
            DataShare.Create(
                SenderId,
                RecipientId,
                PatientDataId,
                "encryptedPayload",
                "encapsulatedKey",
                "signature",
                senderKeyVersion: 1,
                recipientKeyVersion: 1,
                expiresAtUtc);

        [Fact]
        public void Create_WithValidArguments_ReturnsDataShareWithCorrectDefaults()
        {
            var result = CreateTestShare();

            Assert.NotNull(result);
            Assert.Equal(SenderId, result.SenderResearcherId);
            Assert.Equal(RecipientId, result.RecipientResearcherId);
            Assert.Equal(PatientDataId, result.PatientDataId);
            Assert.Equal("encryptedPayload", result.EncryptedPayload);
            Assert.Equal("encapsulatedKey", result.EncapsulatedKey);
            Assert.Equal("signature", result.Signature);
            Assert.Equal(1, result.SenderKeyVersion);
            Assert.Equal(1, result.RecipientKeyVersion);
            Assert.Equal(DataShareStatus.Pending, result.Status);
            Assert.Null(result.AccessedAtUtc);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public void Create_WithValidArguments_RaisesPatientDataSharedEvent()
        {
            var result = CreateTestShare();

            Assert.Single(result.DomainEvents);
            var domainEvent = Assert.IsType<PatientDataSharedEvent>(result.DomainEvents.First());
            Assert.Equal(result.Id, domainEvent.DataShareId);
            Assert.Equal(SenderId, domainEvent.SenderResearcherId);
            Assert.Equal(RecipientId, domainEvent.RecipientResearcherId);
            Assert.Equal(PatientDataId, domainEvent.PatientDataId);
        }

        [Fact]
        public void Create_WithSameSenderAndRecipient_ThrowsArgumentException()
        {
            var sameId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() =>
                DataShare.Create(sameId, sameId, PatientDataId, "payload", "key", "sig", 1, 1));
        }

        [Fact]
        public void Create_WithPastExpiry_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                CreateTestShare(DateTime.UtcNow.AddMinutes(-1)));
        }

        [Fact]
        public void Create_WithNullEncryptedPayload_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                DataShare.Create(SenderId, RecipientId, PatientDataId, null!, "key", "sig", 1, 1));
        }

        [Fact]
        public void Create_WithZeroSenderKeyVersion_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                DataShare.Create(SenderId, RecipientId, PatientDataId, "payload", "key", "sig", 0, 1));
        }

        [Fact]
        public void Accept_WhenPending_SetsStatusToAccepted()
        {
            var share = CreateTestShare();
            share.ClearDomainEvents();

            share.Accept();

            Assert.Equal(DataShareStatus.Accepted, share.Status);
            Assert.NotNull(share.AccessedAtUtc);
            Assert.NotNull(share.UpdatedAtUtc);
        }

        [Fact]
        public void Accept_WhenPending_RaisesDataShareAccessedEvent()
        {
            var share = CreateTestShare();
            share.ClearDomainEvents();

            share.Accept();

            Assert.Single(share.DomainEvents);
            var domainEvent = Assert.IsType<DataShareAccessedEvent>(share.DomainEvents.First());
            Assert.Equal(share.Id, domainEvent.DataShareId);
            Assert.Equal(RecipientId, domainEvent.RecipientResearcherId);
        }

        [Fact]
        public void Accept_WhenAlreadyAccepted_ThrowsInvalidOperationException()
        {
            var share = CreateTestShare();
            share.Accept();

            Assert.Throws<InvalidOperationException>(() => share.Accept());
        }

        [Fact]
        public void Accept_WhenRevoked_ThrowsInvalidOperationException()
        {
            var share = CreateTestShare();
            share.Revoke();

            Assert.Throws<InvalidOperationException>(() => share.Accept());
        }

        [Fact]
        public void Revoke_WhenPending_SetsStatusToRevoked()
        {
            var share = CreateTestShare();
            share.ClearDomainEvents();

            share.Revoke();

            Assert.Equal(DataShareStatus.Revoked, share.Status);
            Assert.NotNull(share.UpdatedAtUtc);
        }

        [Fact]
        public void Revoke_WhenPending_RaisesDataShareRevokedEvent()
        {
            var share = CreateTestShare();
            share.ClearDomainEvents();

            share.Revoke();

            Assert.Single(share.DomainEvents);
            var domainEvent = Assert.IsType<DataShareRevokedEvent>(share.DomainEvents.First());
            Assert.Equal(share.Id, domainEvent.DataShareId);
            Assert.Equal(SenderId, domainEvent.SenderResearcherId);
        }

        [Fact]
        public void Revoke_WhenAlreadyRevoked_ThrowsInvalidOperationException()
        {
            var share = CreateTestShare();
            share.Revoke();

            Assert.Throws<InvalidOperationException>(() => share.Revoke());
        }

        [Fact]
        public void Revoke_WhenAccepted_SetsStatusToRevoked()
        {
            var share = CreateTestShare();
            share.Accept();
            share.ClearDomainEvents();

            share.Revoke();

            Assert.Equal(DataShareStatus.Revoked, share.Status);
        }

        [Fact]
        public void IsExpired_WithNoExpiry_ReturnsFalse()
        {
            var share = CreateTestShare();

            Assert.False(share.IsExpired());
        }

        [Fact]
        public void IsExpired_WithFutureExpiry_ReturnsFalse()
        {
            var share = CreateTestShare(DateTime.UtcNow.AddHours(1));

            Assert.False(share.IsExpired());
        }
    }
}
