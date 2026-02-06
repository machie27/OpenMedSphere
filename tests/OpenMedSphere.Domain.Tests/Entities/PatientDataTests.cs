using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Events;
using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Domain.Tests.Entities
{
    public sealed class PatientDataTests
    {
        [Fact]
        public void Create_WithValidPatientIdentifier_ReturnsPatientDataWithCorrectDefaults()
        {
            PatientIdentifier identifier = PatientIdentifier.Generate();

            PatientData result = PatientData.Create(identifier);

            Assert.NotNull(result);
            Assert.Equal(identifier, result.PatientId);
            Assert.False(result.IsAnonymized);
            Assert.True(result.CreatedAtUtc <= DateTime.UtcNow);
            Assert.True(result.CollectedAtUtc <= DateTime.UtcNow);
            Assert.Empty(result.SecondaryDiagnoses);
            Assert.Empty(result.SecondaryDiagnosisCodes);
            Assert.Empty(result.Medications);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public void Create_WithValidPatientIdentifier_RaisesPatientDataCreatedEvent()
        {
            PatientIdentifier identifier = PatientIdentifier.Generate();

            PatientData result = PatientData.Create(identifier);

            Assert.Single(result.DomainEvents);
            PatientDataCreatedEvent domainEvent = Assert.IsType<PatientDataCreatedEvent>(result.DomainEvents.First());
            Assert.Equal(result.Id, domainEvent.PatientDataId);
        }

        [Fact]
        public void UpdateDemographics_WithValidValues_SetsProperties()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());

            patient.UpdateDemographics(1990, "Male", "Northeast");

            Assert.Equal(1990, patient.YearOfBirth);
            Assert.Equal("Male", patient.Gender);
            Assert.Equal("Northeast", patient.Region);
            Assert.NotNull(patient.UpdatedAtUtc);
        }

        [Fact]
        public void UpdateDemographics_WithNullValues_SetsPropertiesToNull()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());
            patient.UpdateDemographics(1990, "Male", "Northeast");

            patient.UpdateDemographics(null, null, null);

            Assert.Null(patient.YearOfBirth);
            Assert.Null(patient.Gender);
            Assert.Null(patient.Region);
        }

        [Fact]
        public void UpdateDemographics_WithYearOfBirthBelow1900_ThrowsArgumentOutOfRangeException()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                patient.UpdateDemographics(1899, "Male", "Northeast"));
        }

        [Fact]
        public void UpdateDemographics_WithYearOfBirthAboveCurrentYear_ThrowsArgumentOutOfRangeException()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());
            int futureYear = DateTime.UtcNow.Year + 1;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                patient.UpdateDemographics(futureYear, "Male", "Northeast"));
        }

        [Fact]
        public void SetPrimaryDiagnosis_WithValidDiagnosis_SetsPrimaryDiagnosis()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());

            patient.SetPrimaryDiagnosis("Essential hypertension");

            Assert.Equal("Essential hypertension", patient.PrimaryDiagnosis);
            Assert.NotNull(patient.UpdatedAtUtc);
        }

        [Fact]
        public void SetPrimaryDiagnosis_WithNull_ThrowsArgumentNullException()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());

            Assert.Throws<ArgumentNullException>(() => patient.SetPrimaryDiagnosis(null!));
        }

        [Fact]
        public void SetPrimaryDiagnosis_WithWhitespace_ThrowsArgumentException()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());

            Assert.Throws<ArgumentException>(() => patient.SetPrimaryDiagnosis("   "));
        }

        [Fact]
        public void SetPrimaryDiagnosisCode_WithValidCode_SetsPrimaryDiagnosisCodeAndSyncsDiagnosis()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());
            MedicalCode code = MedicalCode.Create("BA00", "Essential hypertension", "ICD-11");

            patient.SetPrimaryDiagnosisCode(code);

            Assert.Equal(code, patient.PrimaryDiagnosisCode);
            Assert.Equal("Essential hypertension", patient.PrimaryDiagnosis);
            Assert.NotNull(patient.UpdatedAtUtc);
        }

        [Fact]
        public void AddSecondaryDiagnosis_WithValidDiagnosis_AddsToCollection()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());

            patient.AddSecondaryDiagnosis("Type 2 diabetes");

            Assert.Single(patient.SecondaryDiagnoses);
            Assert.Contains("Type 2 diabetes", patient.SecondaryDiagnoses);
        }

        [Fact]
        public void AddSecondaryDiagnosis_WithDuplicate_DoesNotAddAgain()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());
            patient.AddSecondaryDiagnosis("Type 2 diabetes");

            patient.AddSecondaryDiagnosis("Type 2 diabetes");

            Assert.Single(patient.SecondaryDiagnoses);
        }

        [Fact]
        public void AddSecondaryDiagnosis_WithNull_ThrowsArgumentNullException()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());

            Assert.Throws<ArgumentNullException>(() => patient.AddSecondaryDiagnosis(null!));
        }

        [Fact]
        public void AddSecondaryDiagnosis_WithWhitespace_ThrowsArgumentException()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());

            Assert.Throws<ArgumentException>(() => patient.AddSecondaryDiagnosis("  "));
        }

        [Fact]
        public void RemoveSecondaryDiagnosis_WithExistingDiagnosis_RemovesFromCollection()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());
            patient.AddSecondaryDiagnosis("Type 2 diabetes");

            patient.RemoveSecondaryDiagnosis("Type 2 diabetes");

            Assert.Empty(patient.SecondaryDiagnoses);
        }

        [Fact]
        public void AddMedication_WithValidMedication_AddsToCollection()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());

            patient.AddMedication("Metformin");

            Assert.Single(patient.Medications);
            Assert.Contains("Metformin", patient.Medications);
        }

        [Fact]
        public void AddMedication_WithDuplicate_DoesNotAddAgain()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());
            patient.AddMedication("Metformin");

            patient.AddMedication("Metformin");

            Assert.Single(patient.Medications);
        }

        [Fact]
        public void AddMedication_WithNull_ThrowsArgumentNullException()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());

            Assert.Throws<ArgumentNullException>(() => patient.AddMedication(null!));
        }

        [Fact]
        public void AddMedication_WithWhitespace_ThrowsArgumentException()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());

            Assert.Throws<ArgumentException>(() => patient.AddMedication("  "));
        }

        [Fact]
        public void RemoveMedication_WithExistingMedication_RemovesFromCollection()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());
            patient.AddMedication("Metformin");

            patient.RemoveMedication("Metformin");

            Assert.Empty(patient.Medications);
        }

        [Fact]
        public void MarkAsAnonymized_WithValidPolicyId_SetsAnonymizationProperties()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());
            Guid policyId = Guid.NewGuid();

            patient.MarkAsAnonymized(policyId);

            Assert.True(patient.IsAnonymized);
            Assert.Equal(policyId, patient.AnonymizationPolicyId);
            Assert.NotNull(patient.AnonymizedAtUtc);
            Assert.NotNull(patient.UpdatedAtUtc);
        }

        [Fact]
        public void MarkAsAnonymized_WithValidPolicyId_RaisesPatientDataAnonymizedEvent()
        {
            PatientData patient = PatientData.Create(PatientIdentifier.Generate());
            patient.ClearDomainEvents();
            Guid policyId = Guid.NewGuid();

            patient.MarkAsAnonymized(policyId);

            Assert.Single(patient.DomainEvents);
            PatientDataAnonymizedEvent domainEvent = Assert.IsType<PatientDataAnonymizedEvent>(patient.DomainEvents.First());
            Assert.Equal(patient.Id, domainEvent.PatientDataId);
            Assert.Equal(policyId, domainEvent.PolicyId);
        }
    }
}
