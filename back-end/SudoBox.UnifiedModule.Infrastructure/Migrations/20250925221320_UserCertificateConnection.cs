using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudoBox.UnifiedModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserCertificateConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "signed_by_id",
                schema: "unified",
                table: "certificates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "certificate_user",
                schema: "unified",
                columns: table => new
                {
                    my_certificates_serial_number = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_certificate_user", x => new { x.my_certificates_serial_number, x.user_id });
                    table.ForeignKey(
                        name: "fk_certificate_user_certificates_my_certificates_serial_number",
                        column: x => x.my_certificates_serial_number,
                        principalSchema: "unified",
                        principalTable: "certificates",
                        principalColumn: "serial_number",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_certificate_user_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "unified",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_certificates_signed_by_id",
                schema: "unified",
                table: "certificates",
                column: "signed_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_certificate_user_user_id",
                schema: "unified",
                table: "certificate_user",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_certificates_users_signed_by_id",
                schema: "unified",
                table: "certificates",
                column: "signed_by_id",
                principalSchema: "unified",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_certificates_users_signed_by_id",
                schema: "unified",
                table: "certificates");

            migrationBuilder.DropTable(
                name: "certificate_user",
                schema: "unified");

            migrationBuilder.DropIndex(
                name: "ix_certificates_signed_by_id",
                schema: "unified",
                table: "certificates");

            migrationBuilder.DropColumn(
                name: "signed_by_id",
                schema: "unified",
                table: "certificates");
        }
    }
}
