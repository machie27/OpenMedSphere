using OpenMedSphere.Domain.Entities;
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

            var result = Researcher.Create("Dr. Smith", "smith@university.edu", "MIT", keys);

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
        public void Create_WithNullName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Researcher.Create(null!, "email@test.com", "MIT", CreateTestKeys()));
        }

        [Fact]
        public void Create_WithWhitespaceName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                Researcher.Create("  ", "email@test.com", "MIT", CreateTestKeys()));
        }

        [Fact]
        public void Create_WithNullEmail_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Researcher.Create("Dr. Smith", null!, "MIT", CreateTestKeys()));
        }

        [Fact]
        public void Create_WithNullInstitution_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Researcher.Create("Dr. Smith", "email@test.com", null!, CreateTestKeys()));
        }

        [Fact]
        public void Create_WithNullPublicKeys_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Researcher.Create("Dr. Smith", "email@test.com", "MIT", null!));
        }

        [Fact]
        public void RotateKeys_WithHigherVersion_UpdatesKeys()
        {
            var researcher = Researcher.Create("Dr. Smith", "email@test.com", "MIT", CreateTestKeys(1));
            var newKeys = CreateTestKeys(2);

            researcher.RotateKeys(newKeys);

            Assert.Equal(newKeys, researcher.PublicKeys);
            Assert.Equal(2, researcher.PublicKeys.KeyVersion);
            Assert.NotNull(researcher.UpdatedAtUtc);
        }

        [Fact]
        public void RotateKeys_WithSameVersion_ThrowsArgumentException()
        {
            var researcher = Researcher.Create("Dr. Smith", "email@test.com", "MIT", CreateTestKeys(1));

            Assert.Throws<ArgumentException>(() =>
                researcher.RotateKeys(CreateTestKeys(1)));
        }

        [Fact]
        public void RotateKeys_WithLowerVersion_ThrowsArgumentException()
        {
            var researcher = Researcher.Create("Dr. Smith", "email@test.com", "MIT", CreateTestKeys(2));

            Assert.Throws<ArgumentException>(() =>
                researcher.RotateKeys(CreateTestKeys(1)));
        }

        [Fact]
        public void RotateKeys_WithNull_ThrowsArgumentNullException()
        {
            var researcher = Researcher.Create("Dr. Smith", "email@test.com", "MIT", CreateTestKeys());

            Assert.Throws<ArgumentNullException>(() =>
                researcher.RotateKeys(null!));
        }

        [Fact]
        public void UpdateProfile_WithValidValues_UpdatesProperties()
        {
            var researcher = Researcher.Create("Dr. Smith", "email@test.com", "MIT", CreateTestKeys());

            researcher.UpdateProfile("Dr. Jones", "jones@harvard.edu", "Harvard");

            Assert.Equal("Dr. Jones", researcher.Name);
            Assert.Equal("jones@harvard.edu", researcher.Email);
            Assert.Equal("Harvard", researcher.Institution);
            Assert.NotNull(researcher.UpdatedAtUtc);
        }

        [Fact]
        public void UpdateProfile_WithNullName_ThrowsArgumentException()
        {
            var researcher = Researcher.Create("Dr. Smith", "email@test.com", "MIT", CreateTestKeys());

            Assert.Throws<ArgumentNullException>(() =>
                researcher.UpdateProfile(null!, "email@test.com", "MIT"));
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            var researcher = Researcher.Create("Dr. Smith", "email@test.com", "MIT", CreateTestKeys());

            researcher.Deactivate();

            Assert.False(researcher.IsActive);
            Assert.NotNull(researcher.UpdatedAtUtc);
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            var researcher = Researcher.Create("Dr. Smith", "email@test.com", "MIT", CreateTestKeys());
            researcher.Deactivate();

            researcher.Activate();

            Assert.True(researcher.IsActive);
        }
    }
}
