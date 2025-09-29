using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudoBox.UnifiedModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEncryption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "master_keys",
                schema: "unified",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    encrypted_key = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_keys",
                schema: "unified",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    encrypted_key = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_keys", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_keys_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "unified",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "master_keys",
                schema: "unified");

            migrationBuilder.DropTable(
                name: "user_keys",
                schema: "unified");
        }
    }
}
