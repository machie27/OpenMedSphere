using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Domain.Tests.ValueObjects
{
    public sealed class MedicalCodeTests
    {
        [Fact]
        public void Create_WithValidParameters_ReturnsMedicalCode()
        {
            MedicalCode result = MedicalCode.Create("BA00", "Essential hypertension", "ICD-11");

            Assert.NotNull(result);
            Assert.Equal("BA00", result.Code);
            Assert.Equal("Essential hypertension", result.DisplayName);
            Assert.Equal("ICD-11", result.CodingSystem);
            Assert.Null(result.EntityUri);
        }

        [Fact]
        public void Create_WithEntityUri_SetsEntityUri()
        {
            MedicalCode result = MedicalCode.Create(
                "BA00",
                "Essential hypertension",
                "ICD-11",
                "http://id.who.int/icd/entity/1234567890");

            Assert.Equal("http://id.who.int/icd/entity/1234567890", result.EntityUri);
        }

        [Fact]
        public void Create_WithNullEntityUri_SetsEntityUriToNull()
        {
            MedicalCode result = MedicalCode.Create("BA00", "Essential hypertension", "ICD-11", null);

            Assert.Null(result.EntityUri);
        }

        [Fact]
        public void Create_WithNullCode_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                MedicalCode.Create(null!, "Essential hypertension", "ICD-11"));
        }

        [Fact]
        public void Create_WithWhitespaceCode_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                MedicalCode.Create("  ", "Essential hypertension", "ICD-11"));
        }

        [Fact]
        public void Create_WithNullDisplayName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                MedicalCode.Create("BA00", null!, "ICD-11"));
        }

        [Fact]
        public void Create_WithWhitespaceDisplayName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                MedicalCode.Create("BA00", "  ", "ICD-11"));
        }

        [Fact]
        public void Create_WithNullCodingSystem_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                MedicalCode.Create("BA00", "Essential hypertension", null!));
        }

        [Fact]
        public void Create_WithWhitespaceCodingSystem_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                MedicalCode.Create("BA00", "Essential hypertension", "  "));
        }

        [Fact]
        public void Equals_TwoCodesWithSameValues_ReturnsTrue()
        {
            MedicalCode first = MedicalCode.Create("BA00", "Essential hypertension", "ICD-11");
            MedicalCode second = MedicalCode.Create("BA00", "Essential hypertension", "ICD-11");

            Assert.Equal(first, second);
        }

        [Fact]
        public void Equals_TwoCodesWithDifferentValues_ReturnsFalse()
        {
            MedicalCode first = MedicalCode.Create("BA00", "Essential hypertension", "ICD-11");
            MedicalCode second = MedicalCode.Create("5A11", "Type 2 diabetes mellitus", "ICD-11");

            Assert.NotEqual(first, second);
        }
    }
}
