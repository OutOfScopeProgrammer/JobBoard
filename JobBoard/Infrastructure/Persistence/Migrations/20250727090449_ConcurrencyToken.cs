using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobBoard.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcurrencyToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ConcurrencyToken",
                table: "Users",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "ConcurrencyToken",
                table: "Jobs",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "ConcurrencyToken",
                table: "Cvs",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "ConcurrencyToken",
                table: "Applications",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcurrencyToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ConcurrencyToken",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ConcurrencyToken",
                table: "Cvs");

            migrationBuilder.DropColumn(
                name: "ConcurrencyToken",
                table: "Applications");
        }
    }
}
