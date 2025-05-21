using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookTradesProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrocasRequiredPatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trocas_Livros_LivroDado_FKId",
                table: "Trocas");

            migrationBuilder.DropForeignKey(
                name: "FK_Trocas_Livros_LivroRecebido_FKId",
                table: "Trocas");

            migrationBuilder.AlterColumn<string>(
                name: "Vendedor",
                table: "Trocas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "LivroRecebido_FKId",
                table: "Trocas",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "LivroRecebido",
                table: "Trocas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "LivroDado_FKId",
                table: "Trocas",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Comprador",
                table: "Trocas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Livros",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "DonoId",
                table: "Livros",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Trocas_Livros_LivroDado_FKId",
                table: "Trocas",
                column: "LivroDado_FKId",
                principalTable: "Livros",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trocas_Livros_LivroRecebido_FKId",
                table: "Trocas",
                column: "LivroRecebido_FKId",
                principalTable: "Livros",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trocas_Livros_LivroDado_FKId",
                table: "Trocas");

            migrationBuilder.DropForeignKey(
                name: "FK_Trocas_Livros_LivroRecebido_FKId",
                table: "Trocas");

            migrationBuilder.AlterColumn<string>(
                name: "Vendedor",
                table: "Trocas",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LivroRecebido_FKId",
                table: "Trocas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LivroRecebido",
                table: "Trocas",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LivroDado_FKId",
                table: "Trocas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comprador",
                table: "Trocas",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Livros",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DonoId",
                table: "Livros",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trocas_Livros_LivroDado_FKId",
                table: "Trocas",
                column: "LivroDado_FKId",
                principalTable: "Livros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trocas_Livros_LivroRecebido_FKId",
                table: "Trocas",
                column: "LivroRecebido_FKId",
                principalTable: "Livros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
