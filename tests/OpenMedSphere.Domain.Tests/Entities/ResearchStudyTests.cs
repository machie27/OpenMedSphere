using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Events;
using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Domain.Tests.Entities
{
    public sealed class ResearchStudyTests
    {
        private static StudyCode DefaultStudyCode => StudyCode.Create("STUDY-001");
        private static DateRange DefaultStudyPeriod => DateRange.Create(
            DateTime.UtcNow,
            DateTime.UtcNow.AddYears(1));
        private static Guid DefaultPolicyId => Guid.NewGuid();

        private static ResearchStudy CreateDefaultStudy(string? title = null)
        {
            return ResearchStudy.Create(
                DefaultStudyCode,
                title ?? "Test Study Title",
                "Dr. Smith",
                "MIT",
                DefaultStudyPeriod,
                DefaultPolicyId,
                "A test study description");
        }

        [Fact]
        public void Create_WithValidParameters_ReturnsResearchStudyWithCorrectDefaults()
        {
            StudyCode code = StudyCode.Create("CARDIO-01");
            DateRange period = DefaultStudyPeriod;
            Guid policyId = Guid.NewGuid();

            ResearchStudy result = ResearchStudy.Create(
                code,
                "Cardiovascular Study",
                "Dr. Johnson",
                "Harvard Medical",
                period,
                policyId,
                "Study of heart conditions");

            Assert.NotNull(result);
            Assert.Equal(code, result.Code);
            Assert.Equal("Cardiovascular Study", result.Title);
            Assert.Equal("Dr. Johnson", result.PrincipalInvestigator);
            Assert.Equal("Harvard Medical", result.Institution);
            Assert.Equal(period, result.StudyPeriod);
            Assert.Equal(policyId, result.AnonymizationPolicyId);
            Assert.Equal("Study of heart conditions", result.Description);
            Assert.True(result.IsActive);
            Assert.True(result.CreatedAtUtc <= DateTime.UtcNow);
            Assert.Empty(result.PatientDataIds);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public void Create_WithValidParameters_RaisesResearchStudyCreatedEvent()
        {
            StudyCode code = StudyCode.Create("EVT-001");

            ResearchStudy result = ResearchStudy.Create(
                code,
                "Event Study",
                "Dr. Lee",
                "Stanford",
                DefaultStudyPeriod,
                DefaultPolicyId);

            Assert.Single(result.DomainEvents);
            ResearchStudyCreatedEvent domainEvent = Assert.IsType<ResearchStudyCreatedEvent>(result.DomainEvents.First());
            Assert.Equal(result.Id, domainEvent.StudyId);
            Assert.Equal("EVT-001", domainEvent.StudyCode);
        }

        [Fact]
        public void Create_WithNullTitle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ResearchStudy.Create(
                    DefaultStudyCode,
                    null!,
                    "Dr. Smith",
                    "MIT",
                    DefaultStudyPeriod,
                    DefaultPolicyId));
        }

        [Fact]
        public void Create_WithWhitespaceTitle_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ResearchStudy.Create(
                    DefaultStudyCode,
                    "   ",
                    "Dr. Smith",
                    "MIT",
                    DefaultStudyPeriod,
                    DefaultPolicyId));
        }

        [Fact]
        public void Create_WithNullPrincipalInvestigator_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ResearchStudy.Create(
                    DefaultStudyCode,
                    "Test Study",
                    null!,
                    "MIT",
                    DefaultStudyPeriod,
                    DefaultPolicyId));
        }

        [Fact]
        public void Create_WithWhitespacePrincipalInvestigator_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ResearchStudy.Create(
                    DefaultStudyCode,
                    "Test Study",
                    "  ",
                    "MIT",
                    DefaultStudyPeriod,
                    DefaultPolicyId));
        }

        [Fact]
        public void Create_WithNullInstitution_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ResearchStudy.Create(
                    DefaultStudyCode,
                    "Test Study",
                    "Dr. Smith",
                    null!,
                    DefaultStudyPeriod,
                    DefaultPolicyId));
        }

        [Fact]
        public void Create_WithWhitespaceInstitution_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ResearchStudy.Create(
                    DefaultStudyCode,
                    "Test Study",
                    "Dr. Smith",
                    "  ",
                    DefaultStudyPeriod,
                    DefaultPolicyId));
        }

        [Fact]
        public void AddPatientData_WithNewId_AddsToCollection()
        {
            ResearchStudy study = CreateDefaultStudy();
            Guid patientDataId = Guid.NewGuid();

            study.AddPatientData(patientDataId);

            Assert.Single(study.PatientDataIds);
            Assert.Contains(patientDataId, study.PatientDataIds);
            Assert.Equal(1, study.CurrentParticipantCount);
        }

        [Fact]
        public void AddPatientData_WithDuplicateId_DoesNotAddAgain()
        {
            ResearchStudy study = CreateDefaultStudy();
            Guid patientDataId = Guid.NewGuid();
            study.AddPatientData(patientDataId);

            study.AddPatientData(patientDataId);

            Assert.Single(study.PatientDataIds);
        }

        [Fact]
        public void AddPatientData_WhenAtMaxParticipants_ThrowsInvalidOperationException()
        {
            ResearchStudy study = CreateDefaultStudy();
            study.SetMaxParticipants(1);
            study.AddPatientData(Guid.NewGuid());

            Assert.Throws<InvalidOperationException>(() =>
                study.AddPatientData(Guid.NewGuid()));
        }

        [Fact]
        public void RemovePatientData_WithExistingId_RemovesFromCollection()
        {
            ResearchStudy study = CreateDefaultStudy();
            Guid patientDataId = Guid.NewGuid();
            study.AddPatientData(patientDataId);

            study.RemovePatientData(patientDataId);

            Assert.Empty(study.PatientDataIds);
            Assert.Equal(0, study.CurrentParticipantCount);
        }

        [Fact]
        public void SetMaxParticipants_WithValueAboveCurrentCount_SetsMaxParticipants()
        {
            ResearchStudy study = CreateDefaultStudy();
            study.AddPatientData(Guid.NewGuid());

            study.SetMaxParticipants(5);

            Assert.Equal(5, study.MaxParticipants);
        }

        [Fact]
        public void SetMaxParticipants_WithValueBelowCurrentCount_ThrowsArgumentOutOfRangeException()
        {
            ResearchStudy study = CreateDefaultStudy();
            study.AddPatientData(Guid.NewGuid());
            study.AddPatientData(Guid.NewGuid());

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                study.SetMaxParticipants(1));
        }

        [Fact]
        public void Activate_WhenInactive_SetsIsActiveToTrue()
        {
            ResearchStudy study = CreateDefaultStudy();
            study.Deactivate();

            study.Activate();

            Assert.True(study.IsActive);
            Assert.NotNull(study.UpdatedAtUtc);
        }

        [Fact]
        public void Deactivate_WhenActive_SetsIsActiveToFalse()
        {
            ResearchStudy study = CreateDefaultStudy();

            study.Deactivate();

            Assert.False(study.IsActive);
            Assert.NotNull(study.UpdatedAtUtc);
        }

        [Fact]
        public void CanAcceptParticipants_WhenInactive_ReturnsFalse()
        {
            ResearchStudy study = CreateDefaultStudy();
            study.Deactivate();

            bool result = study.CanAcceptParticipants();

            Assert.False(result);
        }

        [Fact]
        public void CanAcceptParticipants_WhenAtCapacity_ReturnsFalse()
        {
            ResearchStudy study = CreateDefaultStudy();
            study.SetMaxParticipants(1);
            study.AddPatientData(Guid.NewGuid());

            bool result = study.CanAcceptParticipants();

            Assert.False(result);
        }

        [Fact]
        public void CanAcceptParticipants_WhenActiveAndBelowCapacity_ReturnsTrue()
        {
            ResearchStudy study = CreateDefaultStudy();
            study.SetMaxParticipants(5);
            study.AddPatientData(Guid.NewGuid());

            bool result = study.CanAcceptParticipants();

            Assert.True(result);
        }

        [Fact]
        public void CanAcceptParticipants_WhenActiveAndNoMaxSet_ReturnsTrue()
        {
            ResearchStudy study = CreateDefaultStudy();

            bool result = study.CanAcceptParticipants();

            Assert.True(result);
        }
    }
}
