using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using OpenMedSphere.Domain.ValueObjects;

#nullable disable

namespace OpenMedSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnonymizationPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GeneralizeDateOfBirth = table.Column<bool>(type: "boolean", nullable: false),
                    GeneralizeLocation = table.Column<bool>(type: "boolean", nullable: false),
                    SuppressRareDiagnoses = table.Column<bool>(type: "boolean", nullable: false),
                    KAnonymityThreshold = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymizationPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    YearOfBirth = table.Column<int>(type: "integer", nullable: true),
                    Gender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Region = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PrimaryDiagnosis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PrimaryDiagnosisCode_Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PrimaryDiagnosisCode_DisplayName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PrimaryDiagnosisCode_CodingSystem = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PrimaryDiagnosisCode_EntityUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SecondaryDiagnoses = table.Column<string>(type: "jsonb", nullable: false),
                    SecondaryDiagnosisCodes = table.Column<List<MedicalCode>>(type: "jsonb", nullable: false),
                    Medications = table.Column<string>(type: "jsonb", nullable: false),
                    ClinicalNotes = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    AnonymizationPolicyId = table.Column<Guid>(type: "uuid", nullable: true),
                    CollectedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AnonymizedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAnonymized = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PatientIdentifier = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResearchStudies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    PrincipalInvestigator = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Institution = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    AnonymizationPolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientDataIds = table.Column<string>(type: "jsonb", nullable: false),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: true),
                    ResearchArea = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StudyCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StudyPeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StudyPeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchStudies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnonymizationPolicies_IsActive",
                table: "AnonymizationPolicies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PatientData_IsAnonymized",
                table: "PatientData",
                column: "IsAnonymized");

            migrationBuilder.CreateIndex(
                name: "IX_PatientData_PrimaryDiagnosis",
                table: "PatientData",
                column: "PrimaryDiagnosis");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchStudies_IsActive",
                table: "ResearchStudies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchStudies_ResearchArea",
                table: "ResearchStudies",
                column: "ResearchArea");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonymizationPolicies");

            migrationBuilder.DropTable(
                name: "PatientData");

            migrationBuilder.DropTable(
                name: "ResearchStudies");
        }
    }
}
