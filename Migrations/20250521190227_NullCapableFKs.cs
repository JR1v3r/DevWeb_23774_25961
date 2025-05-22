using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevWeb_23774_25961.Migrations
{
    /// <inheritdoc />
    public partial class NullCapableFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livros_AspNetUsers_UserId",
                table: "Livros");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Livros",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Livros_AspNetUsers_UserId",
                table: "Livros",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livros_AspNetUsers_UserId",
                table: "Livros");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Livros",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Livros_AspNetUsers_UserId",
                table: "Livros",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
