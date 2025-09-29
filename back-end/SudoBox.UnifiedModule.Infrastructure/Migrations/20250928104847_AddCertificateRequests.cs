using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudoBox.UnifiedModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "encoded_value",
                schema: "unified",
                table: "certificates",
                type: "character varying(65536)",
                maxLength: 65536,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(65536)",
                oldMaxLength: 65536,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "encoded_value",
                schema: "unified",
                table: "certificates",
                type: "character varying(65536)",
                maxLength: 65536,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(65536)",
                oldMaxLength: 65536);
        }
    }
}
