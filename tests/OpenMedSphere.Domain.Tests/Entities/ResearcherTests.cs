using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Events;
using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Domain.Tests.Entities
{
    public sealed class ResearcherTests
    {
        private static PublicKeySet CreateTestKeys(int version = 1) =>
            PublicKeySet.Create("mlKem", "mlDsa", "x25519", "ecdsa", version);

        [Fact]
        public void Create_WithValidArguments_ReturnsResearcherWithCorrectDefaults()
        {
            var keys = CreateTestKeys();

            var result = Researcher.Create("ext-1", "Dr. Smith", "smith@university.edu", "MIT", keys);

            Assert.NotNull(result);
            Assert.Equal("Dr. Smith", result.Name);
            Assert.Equal("smith@university.edu", result.Email);
            Assert.Equal("MIT", result.Institution);
            Assert.Equal(keys, result.PublicKeys);
            Assert.True(result.IsActive);
            Assert.True(result.CreatedAtUtc <= DateTime.UtcNow);
            Assert.Null(result.UpdatedAtUtc);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public void Create_WithNullExternalId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Researcher.Create(null!, "Dr. Smith", "email@test.com", "MIT", CreateTestKeys()));
        }

        [Fact]
        public void Create_WithNullName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Researcher.Create("ext-1", null!, "email@test.com", "MIT", CreateTestKeys()));
        }

        [Fact]
        public void Create_WithWhitespaceName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                Researcher.Create("ext-1", "  ", "email@test.com", "MIT", CreateTestKeys()));
        }

        [Fact]
        public void Create_WithNullEmail_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Researcher.Create("ext-1", "Dr. Smith", null!, "MIT", CreateTestKeys()));
        }

        [Fact]
        public void Create_WithNullInstitution_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Researcher.Create("ext-1", "Dr. Smith", "email@test.com", null!, CreateTestKeys()));
        }

        [Fact]
        public void Create_WithNullPublicKeys_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", null!));
        }

        [Fact]
        public void RotateKeys_WithHigherVersion_UpdatesKeys()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys(1));
            var newKeys = CreateTestKeys(2);

            researcher.RotateKeys(newKeys);

            Assert.Equal(newKeys, researcher.PublicKeys);
            Assert.Equal(2, researcher.PublicKeys.KeyVersion);
            Assert.NotNull(researcher.UpdatedAtUtc);
        }

        [Fact]
        public void RotateKeys_WithHigherVersion_RaisesKeyRotatedEvent()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys(1));
            researcher.ClearDomainEvents();

            researcher.RotateKeys(CreateTestKeys(2));

            Assert.Single(researcher.DomainEvents);
            var domainEvent = Assert.IsType<ResearcherKeyRotatedEvent>(researcher.DomainEvents.First());
            Assert.Equal(researcher.Id, domainEvent.ResearcherId);
            Assert.Equal(1, domainEvent.OldKeyVersion);
            Assert.Equal(2, domainEvent.NewKeyVersion);
        }

        [Fact]
        public void RotateKeys_WithSameVersion_ThrowsArgumentException()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys(1));

            Assert.Throws<ArgumentException>(() =>
                researcher.RotateKeys(CreateTestKeys(1)));
        }

        [Fact]
        public void RotateKeys_WithLowerVersion_ThrowsArgumentException()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys(2));

            Assert.Throws<ArgumentException>(() =>
                researcher.RotateKeys(CreateTestKeys(1)));
        }

        [Fact]
        public void RotateKeys_WithNull_ThrowsArgumentNullException()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());

            Assert.Throws<ArgumentNullException>(() =>
                researcher.RotateKeys(null!));
        }

        [Fact]
        public void UpdateProfile_WithValidValues_UpdatesProperties()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());

            researcher.UpdateProfile("Dr. Jones", "jones@harvard.edu", "Harvard");

            Assert.Equal("Dr. Jones", researcher.Name);
            Assert.Equal("jones@harvard.edu", researcher.Email);
            Assert.Equal("Harvard", researcher.Institution);
            Assert.NotNull(researcher.UpdatedAtUtc);
        }

        [Fact]
        public void UpdateProfile_WithValidValues_RaisesProfileUpdatedEvent()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());
            researcher.ClearDomainEvents();

            researcher.UpdateProfile("Dr. Jones", "jones@harvard.edu", "Harvard");

            Assert.Single(researcher.DomainEvents);
            var domainEvent = Assert.IsType<ResearcherProfileUpdatedEvent>(researcher.DomainEvents.First());
            Assert.Equal(researcher.Id, domainEvent.ResearcherId);
        }

        [Fact]
        public void UpdateProfile_WithNullName_ThrowsArgumentException()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());

            Assert.Throws<ArgumentNullException>(() =>
                researcher.UpdateProfile(null!, "email@test.com", "MIT"));
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());

            researcher.Deactivate();

            Assert.False(researcher.IsActive);
            Assert.NotNull(researcher.UpdatedAtUtc);
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());
            researcher.Deactivate();

            researcher.Activate();

            Assert.True(researcher.IsActive);
        }

        [Fact]
        public void Create_WithValidArguments_RaisesResearcherCreatedEvent()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());

            Assert.Single(researcher.DomainEvents);
            var domainEvent = Assert.IsType<ResearcherCreatedEvent>(researcher.DomainEvents.First());
            Assert.Equal(researcher.Id, domainEvent.ResearcherId);
        }

        [Fact]
        public void RotateKeys_WhenInactive_ThrowsInvalidOperationException()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());
            researcher.Deactivate();

            Assert.Throws<InvalidOperationException>(() =>
                researcher.RotateKeys(CreateTestKeys(2)));
        }

        [Fact]
        public void UpdateProfile_WhenInactive_ThrowsInvalidOperationException()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());
            researcher.Deactivate();

            Assert.Throws<InvalidOperationException>(() =>
                researcher.UpdateProfile("Dr. Jones", "jones@harvard.edu", "Harvard"));
        }

        [Fact]
        public void Deactivate_RaisesResearcherDeactivatedEvent()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());
            researcher.ClearDomainEvents();

            researcher.Deactivate();

            Assert.Single(researcher.DomainEvents);
            Assert.IsType<ResearcherDeactivatedEvent>(researcher.DomainEvents.First());
        }

        [Fact]
        public void Activate_RaisesResearcherActivatedEvent()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());
            researcher.Deactivate();
            researcher.ClearDomainEvents();

            researcher.Activate();

            Assert.Single(researcher.DomainEvents);
            Assert.IsType<ResearcherActivatedEvent>(researcher.DomainEvents.First());
        }

        [Fact]
        public void Deactivate_WhenAlreadyInactive_IsNoOp()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());
            researcher.Deactivate();
            var updatedAt = researcher.UpdatedAtUtc;
            researcher.ClearDomainEvents();

            researcher.Deactivate();

            Assert.Empty(researcher.DomainEvents);
            Assert.Equal(updatedAt, researcher.UpdatedAtUtc);
        }

        [Fact]
        public void Activate_WhenAlreadyActive_IsNoOp()
        {
            var researcher = Researcher.Create("ext-1", "Dr. Smith", "email@test.com", "MIT", CreateTestKeys());
            researcher.ClearDomainEvents();

            researcher.Activate();

            Assert.Empty(researcher.DomainEvents);
        }
    }
}
