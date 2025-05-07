using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevWeb_23774_25961.Data.Migrations
{
    /// <inheritdoc />
    public partial class RestructuredDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersBooks_AspNetUsers_Users_Id",
                table: "UsersBooks");

            migrationBuilder.DropIndex(
                name: "IX_UsersBooks_Users_Id",
                table: "UsersBooks");

            migrationBuilder.RenameColumn(
                name: "Users_Id",
                table: "UsersBooks",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "PreviousOwner",
                table: "Trades",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "CurrentOwner",
                table: "Trades",
                newName: "CreatedBy");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "UsersBooks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UsersBooks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "UsersBooks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "UsersBooks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Trades",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Trades",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Trades",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Books",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Books",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Books",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Books",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Books",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersBooks_CreatedBy",
                table: "UsersBooks",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UsersBooks_UpdatedBy",
                table: "UsersBooks",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_CreatedBy",
                table: "Trades",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_UpdatedBy",
                table: "Trades",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CreatedBy",
                table: "Books",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Books_UpdatedBy",
                table: "Books",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_CreatedBy",
                table: "Books",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_UpdatedBy",
                table: "Books",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_AspNetUsers_CreatedBy",
                table: "Trades",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_AspNetUsers_UpdatedBy",
                table: "Trades",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBooks_AspNetUsers_CreatedBy",
                table: "UsersBooks",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBooks_AspNetUsers_UpdatedBy",
                table: "UsersBooks",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_CreatedBy",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_UpdatedBy",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_AspNetUsers_CreatedBy",
                table: "Trades");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_AspNetUsers_UpdatedBy",
                table: "Trades");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersBooks_AspNetUsers_CreatedBy",
                table: "UsersBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersBooks_AspNetUsers_UpdatedBy",
                table: "UsersBooks");

            migrationBuilder.DropIndex(
                name: "IX_UsersBooks_CreatedBy",
                table: "UsersBooks");

            migrationBuilder.DropIndex(
                name: "IX_UsersBooks_UpdatedBy",
                table: "UsersBooks");

            migrationBuilder.DropIndex(
                name: "IX_Trades_CreatedBy",
                table: "Trades");

            migrationBuilder.DropIndex(
                name: "IX_Trades_UpdatedBy",
                table: "Trades");

            migrationBuilder.DropIndex(
                name: "IX_Books_CreatedBy",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_UpdatedBy",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UsersBooks");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UsersBooks");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "UsersBooks");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "UsersBooks");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "UsersBooks",
                newName: "Users_Id");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Trades",
                newName: "PreviousOwner");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Trades",
                newName: "CurrentOwner");

            migrationBuilder.CreateIndex(
                name: "IX_UsersBooks_Users_Id",
                table: "UsersBooks",
                column: "Users_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBooks_AspNetUsers_Users_Id",
                table: "UsersBooks",
                column: "Users_Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
