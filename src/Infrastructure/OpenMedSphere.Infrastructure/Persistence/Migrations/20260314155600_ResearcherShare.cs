using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenMedSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ResearcherShare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Researchers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Institution = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    PublicKeys_MlKem = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    PublicKeys_MlDsa = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    PublicKeys_X25519 = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    PublicKeys_Ecdsa = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    PublicKeys_KeyVersion = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Researchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataShares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderResearcherId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientResearcherId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    EncryptedPayload = table.Column<string>(type: "text", nullable: false),
                    EncapsulatedKey = table.Column<string>(type: "text", nullable: false),
                    Signature = table.Column<string>(type: "text", nullable: false),
                    SenderKeyVersion = table.Column<int>(type: "integer", nullable: false),
                    RecipientKeyVersion = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SharedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataShares_PatientData_PatientDataId",
                        column: x => x.PatientDataId,
                        principalTable: "PatientData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DataShares_Researchers_RecipientResearcherId",
                        column: x => x.RecipientResearcherId,
                        principalTable: "Researchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DataShares_Researchers_SenderResearcherId",
                        column: x => x.SenderResearcherId,
                        principalTable: "Researchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataShares_PatientDataId",
                table: "DataShares",
                column: "PatientDataId");

            migrationBuilder.CreateIndex(
                name: "IX_DataShares_RecipientResearcherId",
                table: "DataShares",
                column: "RecipientResearcherId");

            migrationBuilder.CreateIndex(
                name: "IX_DataShares_SenderResearcherId",
                table: "DataShares",
                column: "SenderResearcherId");

            migrationBuilder.CreateIndex(
                name: "IX_DataShares_Status",
                table: "DataShares",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Researchers_Email",
                table: "Researchers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Researchers_Institution",
                table: "Researchers",
                column: "Institution");

            migrationBuilder.CreateIndex(
                name: "IX_Researchers_IsActive",
                table: "Researchers",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataShares");

            migrationBuilder.DropTable(
                name: "Researchers");
        }
    }
}
