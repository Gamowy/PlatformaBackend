using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platforma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NameUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Assignments_AssignmentId",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Users_UserId",
                table: "Answer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answer",
                table: "Answer");

            migrationBuilder.RenameTable(
                name: "Answer",
                newName: "Answers");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_UserId",
                table: "Answers",
                newName: "IX_Answers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_AssignmentId",
                table: "Answers",
                newName: "IX_Answers_AssignmentId");

            migrationBuilder.AlterColumn<float>(
                name: "Mark",
                table: "Answers",
                type: "float",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Answers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answers",
                table: "Answers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Assignments_AssignmentId",
                table: "Answers",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Users_UserId",
                table: "Answers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Assignments_AssignmentId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Users_UserId",
                table: "Answers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answers",
                table: "Answers");

            migrationBuilder.RenameTable(
                name: "Answers",
                newName: "Answer");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_UserId",
                table: "Answer",
                newName: "IX_Answer_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_AssignmentId",
                table: "Answer",
                newName: "IX_Answer_AssignmentId");

            migrationBuilder.AlterColumn<float>(
                name: "Mark",
                table: "Answer",
                type: "float",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Answer",
                keyColumn: "Comment",
                keyValue: null,
                column: "Comment",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Answer",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answer",
                table: "Answer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Assignments_AssignmentId",
                table: "Answer",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Users_UserId",
                table: "Answer",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
