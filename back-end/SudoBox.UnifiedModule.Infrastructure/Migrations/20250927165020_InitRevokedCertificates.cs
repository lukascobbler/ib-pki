using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudoBox.UnifiedModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitRevokedCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "certificate_id",
                schema: "unified",
                table: "certificates",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "revoked_certificates",
                schema: "unified",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    revocation_reason = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_revoked_certificates", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_certificates_certificate_id",
                schema: "unified",
                table: "certificates",
                column: "certificate_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_certificates_revoked_certificates_certificate_id",
                schema: "unified",
                table: "certificates",
                column: "certificate_id",
                principalSchema: "unified",
                principalTable: "revoked_certificates",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_certificates_revoked_certificates_certificate_id",
                schema: "unified",
                table: "certificates");

            migrationBuilder.DropTable(
                name: "revoked_certificates",
                schema: "unified");

            migrationBuilder.DropIndex(
                name: "ix_certificates_certificate_id",
                schema: "unified",
                table: "certificates");

            migrationBuilder.DropColumn(
                name: "certificate_id",
                schema: "unified",
                table: "certificates");
        }
    }
}
