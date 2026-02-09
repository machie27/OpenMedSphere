using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OpenMedSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedAnonymizationPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AnonymizationPolicies",
                columns: new[] { "Id", "CreatedAtUtc", "Description", "GeneralizeDateOfBirth", "GeneralizeLocation", "IsActive", "KAnonymityThreshold", "Level", "Name", "SuppressRareDiagnoses", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "No anonymization applied. Data remains in its original form.", false, false, true, null, "None", "No Anonymization", false, null },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Removes direct identifiers such as name, address, and SSN.", false, false, true, null, "Basic", "Basic Anonymization", false, null },
                    { new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Removes direct identifiers and generalizes quasi-identifiers such as date of birth and location.", true, true, true, null, "Standard", "Standard Anonymization", false, null },
                    { new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Applies k-anonymity, generalization, and suppression techniques to prevent re-identification.", true, true, true, 5, "Advanced", "Advanced Anonymization", true, null },
                    { new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Applies differential privacy techniques and maximum generalization for the highest level of privacy protection.", true, true, true, 5, "Full", "Full Anonymization", true, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AnonymizationPolicies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "AnonymizationPolicies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "AnonymizationPolicies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "AnonymizationPolicies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "AnonymizationPolicies",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"));
        }
    }
}
