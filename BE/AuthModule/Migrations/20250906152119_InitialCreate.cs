using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthModule.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    UserType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "PasswordHash", "UpdatedAt", "UserType", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 6, 15, 21, 18, 350, DateTimeKind.Utc).AddTicks(9427), "admin@example.com", true, "$2a$11$qhFeWT/KdefbBkdrRtGjbOS94UVpQhg3xhTG5p8DX2cmhpd8IVNNC", new DateTime(2025, 9, 6, 15, 21, 18, 350, DateTimeKind.Utc).AddTicks(9749), "Admin", "admin" },
                    { 2, new DateTime(2025, 9, 6, 15, 21, 18, 486, DateTimeKind.Utc).AddTicks(4786), "user@example.com", true, "$2a$11$g7ZNuUk.LUXwcuf8xa.7Y.TQH/huPanhJOsL2REPUMuwmk/lgTS.S", new DateTime(2025, 9, 6, 15, 21, 18, 486, DateTimeKind.Utc).AddTicks(4810), "EndUser", "user" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
