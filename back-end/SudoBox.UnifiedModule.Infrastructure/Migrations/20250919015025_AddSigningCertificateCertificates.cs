using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudoBox.UnifiedModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSigningCertificateCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "signing_certificate_serial_number",
                schema: "unified",
                table: "certificates",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_certificates_signing_certificate_serial_number",
                schema: "unified",
                table: "certificates",
                column: "signing_certificate_serial_number");

            migrationBuilder.AddForeignKey(
                name: "fk_certificates_certificates_signing_certificate_serial_number",
                schema: "unified",
                table: "certificates",
                column: "signing_certificate_serial_number",
                principalSchema: "unified",
                principalTable: "certificates",
                principalColumn: "serial_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_certificates_certificates_signing_certificate_serial_number",
                schema: "unified",
                table: "certificates");

            migrationBuilder.DropIndex(
                name: "ix_certificates_signing_certificate_serial_number",
                schema: "unified",
                table: "certificates");

            migrationBuilder.DropColumn(
                name: "signing_certificate_serial_number",
                schema: "unified",
                table: "certificates");
        }
    }
}
