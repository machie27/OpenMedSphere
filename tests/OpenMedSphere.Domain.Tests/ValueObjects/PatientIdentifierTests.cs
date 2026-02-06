using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Domain.Tests.ValueObjects
{
    public sealed class PatientIdentifierTests
    {
        [Fact]
        public void Create_WithValidString_ReturnsPatientIdentifier()
        {
            PatientIdentifier result = PatientIdentifier.Create("PAT-12345");

            Assert.NotNull(result);
            Assert.Equal("PAT-12345", result.Value);
        }

        [Fact]
        public void Create_WithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => PatientIdentifier.Create(null!));
        }

        [Fact]
        public void Create_WithEmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PatientIdentifier.Create(string.Empty));
        }

        [Fact]
        public void Create_WithWhitespace_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PatientIdentifier.Create("   "));
        }

        [Fact]
        public void Generate_ReturnsNonNullIdentifierWithNonEmptyValue()
        {
            PatientIdentifier result = PatientIdentifier.Generate();

            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.Value));
        }

        [Fact]
        public void Equals_TwoIdentifiersWithSameValue_ReturnsTrue()
        {
            PatientIdentifier first = PatientIdentifier.Create("PAT-SAME");
            PatientIdentifier second = PatientIdentifier.Create("PAT-SAME");

            Assert.Equal(first, second);
        }

        [Fact]
        public void Equals_TwoIdentifiersWithDifferentValues_ReturnsFalse()
        {
            PatientIdentifier first = PatientIdentifier.Create("PAT-001");
            PatientIdentifier second = PatientIdentifier.Create("PAT-002");

            Assert.NotEqual(first, second);
        }
    }
}
