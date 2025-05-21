using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookTradesProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixedAppDBContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Livros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", nullable: false),
                    Autor = table.Column<string>(type: "TEXT", nullable: false),
                    ISBN = table.Column<int>(type: "INTEGER", nullable: false),
                    Sinopse = table.Column<string>(type: "TEXT", nullable: false),
                    Capa = table.Column<string>(type: "TEXT", nullable: false),
                    DonoId = table.Column<string>(type: "TEXT", nullable: true),
                    DonoId_FKId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Livros_AspNetUsers_DonoId_FKId",
                        column: x => x.DonoId_FKId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Trocas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LivroDado = table.Column<string>(type: "TEXT", nullable: false),
                    LivroDado_FKId = table.Column<int>(type: "INTEGER", nullable: false),
                    LivroRecebido = table.Column<string>(type: "TEXT", nullable: false),
                    LivroRecebido_FKId = table.Column<int>(type: "INTEGER", nullable: false),
                    Vendedor = table.Column<string>(type: "TEXT", nullable: false),
                    Vendedor_FKId = table.Column<string>(type: "TEXT", nullable: true),
                    Comprador = table.Column<string>(type: "TEXT", nullable: false),
                    Comprador_FKId = table.Column<string>(type: "TEXT", nullable: true),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trocas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trocas_AspNetUsers_Comprador_FKId",
                        column: x => x.Comprador_FKId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Trocas_AspNetUsers_Vendedor_FKId",
                        column: x => x.Vendedor_FKId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Trocas_Livros_LivroDado_FKId",
                        column: x => x.LivroDado_FKId,
                        principalTable: "Livros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trocas_Livros_LivroRecebido_FKId",
                        column: x => x.LivroRecebido_FKId,
                        principalTable: "Livros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Livros_DonoId_FKId",
                table: "Livros",
                column: "DonoId_FKId");

            migrationBuilder.CreateIndex(
                name: "IX_Trocas_Comprador_FKId",
                table: "Trocas",
                column: "Comprador_FKId");

            migrationBuilder.CreateIndex(
                name: "IX_Trocas_LivroDado_FKId",
                table: "Trocas",
                column: "LivroDado_FKId");

            migrationBuilder.CreateIndex(
                name: "IX_Trocas_LivroRecebido_FKId",
                table: "Trocas",
                column: "LivroRecebido_FKId");

            migrationBuilder.CreateIndex(
                name: "IX_Trocas_Vendedor_FKId",
                table: "Trocas",
                column: "Vendedor_FKId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trocas");

            migrationBuilder.DropTable(
                name: "Livros");
        }
    }
}
