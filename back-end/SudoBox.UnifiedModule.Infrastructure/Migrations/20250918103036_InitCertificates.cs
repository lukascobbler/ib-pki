using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudoBox.UnifiedModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "certificates",
                schema: "unified",
                columns: table => new
                {
                    serial_number = table.Column<string>(type: "text", nullable: false),
                    issued_by = table.Column<string>(type: "text", nullable: false),
                    issued_to = table.Column<string>(type: "text", nullable: false),
                    not_before = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    not_after = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    encoded_value = table.Column<string>(type: "character varying(65536)", maxLength: 65536, nullable: true),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false),
                    private_key = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_certificates", x => x.serial_number);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "certificates",
                schema: "unified");
        }
    }
}
