using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudoBox.UnifiedModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedCrlFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_certificates_revoked_certificates_certificate_id",
                schema: "unified",
                table: "certificates");

            migrationBuilder.DropPrimaryKey(
                name: "pk_revoked_certificates",
                schema: "unified",
                table: "revoked_certificates");

            migrationBuilder.DropIndex(
                name: "ix_certificates_certificate_id",
                schema: "unified",
                table: "certificates");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "unified",
                table: "revoked_certificates");

            migrationBuilder.DropColumn(
                name: "certificate_id",
                schema: "unified",
                table: "certificates");

            migrationBuilder.DropColumn(
                name: "is_approved",
                schema: "unified",
                table: "certificates");

            migrationBuilder.AddColumn<string>(
                name: "certificate_serial_number",
                schema: "unified",
                table: "revoked_certificates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_revoked_certificates",
                schema: "unified",
                table: "revoked_certificates",
                column: "certificate_serial_number");

            migrationBuilder.AddForeignKey(
                name: "fk_revoked_certificates_certificates_certificate_serial_number",
                schema: "unified",
                table: "revoked_certificates",
                column: "certificate_serial_number",
                principalSchema: "unified",
                principalTable: "certificates",
                principalColumn: "serial_number",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_revoked_certificates_certificates_certificate_serial_number",
                schema: "unified",
                table: "revoked_certificates");

            migrationBuilder.DropPrimaryKey(
                name: "pk_revoked_certificates",
                schema: "unified",
                table: "revoked_certificates");

            migrationBuilder.DropColumn(
                name: "certificate_serial_number",
                schema: "unified",
                table: "revoked_certificates");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "unified",
                table: "revoked_certificates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "certificate_id",
                schema: "unified",
                table: "certificates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_approved",
                schema: "unified",
                table: "certificates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "pk_revoked_certificates",
                schema: "unified",
                table: "revoked_certificates",
                column: "id");

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
    }
}
