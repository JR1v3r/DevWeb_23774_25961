using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevWeb_23774_25961.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabaseCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", nullable: false),
                    Autor = table.Column<string>(type: "TEXT", nullable: false),
                    Descrição = table.Column<string>(type: "TEXT", nullable: false),
                    ISBN = table.Column<int>(type: "INTEGER", nullable: false),
                    CapaPath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersBooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Users_Id = table.Column<string>(type: "TEXT", nullable: false),
                    Books_Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersBooks_AspNetUsers_Users_Id",
                        column: x => x.Users_Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersBooks_Books_Books_Id",
                        column: x => x.Books_Id,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsersBooks_Id = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviousOwner = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentOwner = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_UsersBooks_UsersBooks_Id",
                        column: x => x.UsersBooks_Id,
                        principalTable: "UsersBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trades_UsersBooks_Id",
                table: "Trades",
                column: "UsersBooks_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UsersBooks_Books_Id",
                table: "UsersBooks",
                column: "Books_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UsersBooks_Users_Id",
                table: "UsersBooks",
                column: "Users_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "UsersBooks");

            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
