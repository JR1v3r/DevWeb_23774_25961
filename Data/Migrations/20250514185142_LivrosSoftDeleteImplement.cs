using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookTradesProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class LivrosSoftDeleteImplement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Livros",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Livros");
        }
    }
}
