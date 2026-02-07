using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenMedSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SwitchValueObjectsToOwnedTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OldValues = table.Column<string>(type: "jsonb", nullable: true),
                    NewValues = table.Column<string>(type: "jsonb", nullable: true),
                    UserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResearchStudies_AnonymizationPolicyId",
                table: "ResearchStudies",
                column: "AnonymizationPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchStudies_StudyCode",
                table: "ResearchStudies",
                column: "StudyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientData_AnonymizationPolicyId",
                table: "PatientData",
                column: "AnonymizationPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientData_CollectedAtUtc",
                table: "PatientData",
                column: "CollectedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_PatientData_PatientIdentifier",
                table: "PatientData",
                column: "PatientIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_PatientData_Region",
                table: "PatientData",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_EntityType",
                table: "AuditLog",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_EntityType_EntityId",
                table: "AuditLog",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_OccurredAtUtc",
                table: "AuditLog",
                column: "OccurredAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropIndex(
                name: "IX_ResearchStudies_AnonymizationPolicyId",
                table: "ResearchStudies");

            migrationBuilder.DropIndex(
                name: "IX_ResearchStudies_StudyCode",
                table: "ResearchStudies");

            migrationBuilder.DropIndex(
                name: "IX_PatientData_AnonymizationPolicyId",
                table: "PatientData");

            migrationBuilder.DropIndex(
                name: "IX_PatientData_CollectedAtUtc",
                table: "PatientData");

            migrationBuilder.DropIndex(
                name: "IX_PatientData_PatientIdentifier",
                table: "PatientData");

            migrationBuilder.DropIndex(
                name: "IX_PatientData_Region",
                table: "PatientData");
        }
    }
}
