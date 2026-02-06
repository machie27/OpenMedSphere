using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;
using Xunit;

namespace OpenMedSphere.Domain.Tests.Entities
{
    public sealed class AnonymizationPolicyTests
    {
        [Fact]
        public void Create_WithValidParameters_ReturnsPolicyWithCorrectDefaults()
        {
            AnonymizationPolicy result = AnonymizationPolicy.Create(
                "Test Policy",
                AnonymizationLevel.Basic,
                "A basic policy");

            Assert.NotNull(result);
            Assert.Equal("Test Policy", result.Name);
            Assert.Equal(AnonymizationLevel.Basic, result.Level);
            Assert.Equal("A basic policy", result.Description);
            Assert.True(result.IsActive);
            Assert.True(result.CreatedAtUtc <= DateTime.UtcNow);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public void Create_WithStandardLevel_SetsGeneralizationOptions()
        {
            AnonymizationPolicy result = AnonymizationPolicy.Create(
                "Standard Policy",
                AnonymizationLevel.Standard);

            Assert.True(result.GeneralizeDateOfBirth);
            Assert.True(result.GeneralizeLocation);
            Assert.False(result.SuppressRareDiagnoses);
            Assert.Null(result.KAnonymityThreshold);
        }

        [Fact]
        public void Create_WithAdvancedLevel_SetsAdvancedOptions()
        {
            AnonymizationPolicy result = AnonymizationPolicy.Create(
                "Advanced Policy",
                AnonymizationLevel.Advanced);

            Assert.True(result.GeneralizeDateOfBirth);
            Assert.True(result.GeneralizeLocation);
            Assert.True(result.SuppressRareDiagnoses);
            Assert.Equal(5, result.KAnonymityThreshold);
        }

        [Fact]
        public void Create_WithBasicLevel_DoesNotSetGeneralizationOptions()
        {
            AnonymizationPolicy result = AnonymizationPolicy.Create(
                "Basic Policy",
                AnonymizationLevel.Basic);

            Assert.False(result.GeneralizeDateOfBirth);
            Assert.False(result.GeneralizeLocation);
            Assert.False(result.SuppressRareDiagnoses);
            Assert.Null(result.KAnonymityThreshold);
        }

        [Fact]
        public void Create_WithNoneLevel_DoesNotSetAnyOptions()
        {
            AnonymizationPolicy result = AnonymizationPolicy.Create(
                "No Anonymization",
                AnonymizationLevel.None);

            Assert.False(result.GeneralizeDateOfBirth);
            Assert.False(result.GeneralizeLocation);
            Assert.False(result.SuppressRareDiagnoses);
            Assert.Null(result.KAnonymityThreshold);
        }

        [Fact]
        public void Create_WithFullLevel_SetsAllOptions()
        {
            AnonymizationPolicy result = AnonymizationPolicy.Create(
                "Full Policy",
                AnonymizationLevel.Full);

            Assert.True(result.GeneralizeDateOfBirth);
            Assert.True(result.GeneralizeLocation);
            Assert.True(result.SuppressRareDiagnoses);
            Assert.Equal(5, result.KAnonymityThreshold);
        }

        [Fact]
        public void Create_WithNullName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                AnonymizationPolicy.Create(null!, AnonymizationLevel.Standard));
        }

        [Fact]
        public void Create_WithWhitespaceName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                AnonymizationPolicy.Create("   ", AnonymizationLevel.Standard));
        }

        [Fact]
        public void ConfigureKAnonymity_WithValidThreshold_SetsThreshold()
        {
            AnonymizationPolicy policy = AnonymizationPolicy.Create(
                "Test Policy",
                AnonymizationLevel.Advanced);

            policy.ConfigureKAnonymity(10);

            Assert.Equal(10, policy.KAnonymityThreshold);
            Assert.NotNull(policy.UpdatedAtUtc);
        }

        [Fact]
        public void ConfigureKAnonymity_WithMinimumThreshold_SetsThreshold()
        {
            AnonymizationPolicy policy = AnonymizationPolicy.Create(
                "Test Policy",
                AnonymizationLevel.Advanced);

            policy.ConfigureKAnonymity(2);

            Assert.Equal(2, policy.KAnonymityThreshold);
        }

        [Fact]
        public void ConfigureKAnonymity_WithThresholdBelow2_ThrowsArgumentOutOfRangeException()
        {
            AnonymizationPolicy policy = AnonymizationPolicy.Create(
                "Test Policy",
                AnonymizationLevel.Advanced);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                policy.ConfigureKAnonymity(1));
        }

        [Fact]
        public void Activate_WhenInactive_SetsIsActiveToTrue()
        {
            AnonymizationPolicy policy = AnonymizationPolicy.Create(
                "Test Policy",
                AnonymizationLevel.Standard);
            policy.Deactivate();

            policy.Activate();

            Assert.True(policy.IsActive);
            Assert.NotNull(policy.UpdatedAtUtc);
        }

        [Fact]
        public void Deactivate_WhenActive_SetsIsActiveToFalse()
        {
            AnonymizationPolicy policy = AnonymizationPolicy.Create(
                "Test Policy",
                AnonymizationLevel.Standard);

            policy.Deactivate();

            Assert.False(policy.IsActive);
            Assert.NotNull(policy.UpdatedAtUtc);
        }
    }
}
