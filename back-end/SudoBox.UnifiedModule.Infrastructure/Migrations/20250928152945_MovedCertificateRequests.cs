using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudoBox.UnifiedModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MovedCertificateRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "certificate_requests",
                schema: "unified",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    encoded_csr = table.Column<string>(type: "character varying(65536)", maxLength: 65536, nullable: false),
                    requested_for_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_from_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_certificate_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_certificate_requests_users_requested_for_id",
                        column: x => x.requested_for_id,
                        principalSchema: "unified",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_certificate_requests_users_requested_from_id",
                        column: x => x.requested_from_id,
                        principalSchema: "unified",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_certificate_requests_requested_for_id",
                schema: "unified",
                table: "certificate_requests",
                column: "requested_for_id");

            migrationBuilder.CreateIndex(
                name: "ix_certificate_requests_requested_from_id",
                schema: "unified",
                table: "certificate_requests",
                column: "requested_from_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "certificate_requests",
                schema: "unified");
        }
    }
}
