using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Domain.Tests.ValueObjects
{
    public sealed class StudyCodeTests
    {
        [Fact]
        public void Create_WithValidString_ReturnsUppercaseValue()
        {
            StudyCode result = StudyCode.Create("study-abc");

            Assert.NotNull(result);
            Assert.Equal("STUDY-ABC", result.Value);
        }

        [Fact]
        public void Create_WithAlreadyUppercaseString_ReturnsSameValue()
        {
            StudyCode result = StudyCode.Create("STUDY-XYZ");

            Assert.Equal("STUDY-XYZ", result.Value);
        }

        [Fact]
        public void Create_WithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => StudyCode.Create(null!));
        }

        [Fact]
        public void Create_WithEmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => StudyCode.Create(string.Empty));
        }

        [Fact]
        public void Create_WithWhitespace_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => StudyCode.Create("   "));
        }

        [Fact]
        public void Create_WithMoreThan50Characters_ThrowsArgumentException()
        {
            string longCode = new('A', 51);

            Assert.Throws<ArgumentException>(() => StudyCode.Create(longCode));
        }

        [Fact]
        public void Create_WithExactly50Characters_Succeeds()
        {
            string exactCode = new('A', 50);

            StudyCode result = StudyCode.Create(exactCode);

            Assert.Equal(exactCode, result.Value);
        }

        [Fact]
        public void Equals_TwoCodesWithSameValue_ReturnsTrue()
        {
            StudyCode first = StudyCode.Create("CODE-01");
            StudyCode second = StudyCode.Create("code-01");

            Assert.Equal(first, second);
        }
    }
}
