using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudoBox.UnifiedModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNotBeforeNotAfterToCSR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "not_after",
                schema: "unified",
                table: "certificate_requests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "not_before",
                schema: "unified",
                table: "certificate_requests",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "not_after",
                schema: "unified",
                table: "certificate_requests");

            migrationBuilder.DropColumn(
                name: "not_before",
                schema: "unified",
                table: "certificate_requests");
        }
    }
}
