using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platforma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Assignments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Answers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Answers");
        }
    }
}
