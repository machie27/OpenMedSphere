using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.PatientData.Commands.CreatePatientData;
using Xunit;

namespace OpenMedSphere.Application.Tests.PatientData.Commands
{
    public sealed class CreatePatientDataCommandValidatorTests
    {
        private readonly CreatePatientDataCommandValidator _validator = new();

        [Fact]
        public async Task ValidateAsync_EmptySecondaryDiagnosis_ReturnsError()
        {
            var command = new CreatePatientDataCommand
            {
                SecondaryDiagnoses = ["Diabetes", "", "Asthma"]
            };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.SecondaryDiagnoses));
        }

        [Fact]
        public async Task ValidateAsync_WhitespaceSecondaryDiagnosis_ReturnsError()
        {
            var command = new CreatePatientDataCommand
            {
                SecondaryDiagnoses = ["  "]
            };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.SecondaryDiagnoses));
        }

        [Fact]
        public async Task ValidateAsync_EmptyMedication_ReturnsError()
        {
            var command = new CreatePatientDataCommand
            {
                Medications = ["Aspirin", ""]
            };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Medications));
        }

        [Fact]
        public async Task ValidateAsync_ValidSecondaryDiagnoses_ReturnsSuccess()
        {
            var command = new CreatePatientDataCommand
            {
                SecondaryDiagnoses = ["Diabetes", "Asthma"]
            };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_ValidMedications_ReturnsSuccess()
        {
            var command = new CreatePatientDataCommand
            {
                Medications = ["Aspirin", "Metformin"]
            };

            var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

            Assert.True(result.IsValid);
        }
    }
}
