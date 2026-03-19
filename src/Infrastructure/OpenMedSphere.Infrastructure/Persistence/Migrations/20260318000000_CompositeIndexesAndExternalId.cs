using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenMedSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompositeIndexesAndExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Issue 4: Add ExternalId column for one-researcher-per-identity enforcement.
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Researchers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            // Backfill any existing rows with their PK (Guid) as a unique placeholder.
            // This won't match a real JWT NameIdentifier, so backfilled researchers cannot
            // authenticate via the normal flow — acceptable since there is no production data yet.
            migrationBuilder.Sql(
                "UPDATE \"Researchers\" SET \"ExternalId\" = \"Id\"::text WHERE \"ExternalId\" = ''");

            migrationBuilder.CreateIndex(
                name: "IX_Researchers_ExternalId",
                table: "Researchers",
                column: "ExternalId",
                unique: true);

            // Issue 1: Replace single-column FK indexes with composite indexes
            // for paginated list query performance.
            migrationBuilder.DropIndex(
                name: "IX_DataShares_SenderResearcherId",
                table: "DataShares");

            migrationBuilder.DropIndex(
                name: "IX_DataShares_RecipientResearcherId",
                table: "DataShares");

            migrationBuilder.CreateIndex(
                name: "IX_DataShares_SenderResearcherId_SharedAtUtc",
                table: "DataShares",
                columns: new[] { "SenderResearcherId", "SharedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_DataShares_RecipientResearcherId_SharedAtUtc",
                table: "DataShares",
                columns: new[] { "RecipientResearcherId", "SharedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DataShares_SenderResearcherId_SharedAtUtc",
                table: "DataShares");

            migrationBuilder.DropIndex(
                name: "IX_DataShares_RecipientResearcherId_SharedAtUtc",
                table: "DataShares");

            migrationBuilder.CreateIndex(
                name: "IX_DataShares_SenderResearcherId",
                table: "DataShares",
                column: "SenderResearcherId");

            migrationBuilder.CreateIndex(
                name: "IX_DataShares_RecipientResearcherId",
                table: "DataShares",
                column: "RecipientResearcherId");

            migrationBuilder.DropIndex(
                name: "IX_Researchers_ExternalId",
                table: "Researchers");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Researchers");
        }
    }
}
