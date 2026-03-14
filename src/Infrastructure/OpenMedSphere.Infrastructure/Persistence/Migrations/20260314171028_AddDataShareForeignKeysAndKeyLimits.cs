using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenMedSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDataShareForeignKeysAndKeyLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PublicKeys_X25519",
                table: "Researchers",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PublicKeys_MlKem",
                table: "Researchers",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PublicKeys_MlDsa",
                table: "Researchers",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PublicKeys_Ecdsa",
                table: "Researchers",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_DataShares_PatientData_PatientDataId",
                table: "DataShares",
                column: "PatientDataId",
                principalTable: "PatientData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DataShares_Researchers_RecipientResearcherId",
                table: "DataShares",
                column: "RecipientResearcherId",
                principalTable: "Researchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DataShares_Researchers_SenderResearcherId",
                table: "DataShares",
                column: "SenderResearcherId",
                principalTable: "Researchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataShares_PatientData_PatientDataId",
                table: "DataShares");

            migrationBuilder.DropForeignKey(
                name: "FK_DataShares_Researchers_RecipientResearcherId",
                table: "DataShares");

            migrationBuilder.DropForeignKey(
                name: "FK_DataShares_Researchers_SenderResearcherId",
                table: "DataShares");

            migrationBuilder.AlterColumn<string>(
                name: "PublicKeys_X25519",
                table: "Researchers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000);

            migrationBuilder.AlterColumn<string>(
                name: "PublicKeys_MlKem",
                table: "Researchers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000);

            migrationBuilder.AlterColumn<string>(
                name: "PublicKeys_MlDsa",
                table: "Researchers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000);

            migrationBuilder.AlterColumn<string>(
                name: "PublicKeys_Ecdsa",
                table: "Researchers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000);
        }
    }
}
