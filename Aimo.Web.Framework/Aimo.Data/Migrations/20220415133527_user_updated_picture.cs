using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aimo.Data.Migrations
{
    public partial class user_updated_picture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PictureUser");

            migrationBuilder.DropColumn(
                name: "Binary",
                table: "Picture");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Picture",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Picture",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Picture_UserId",
                table: "Picture",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Picture_User_UserId",
                table: "Picture",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Picture_User_UserId",
                table: "Picture");

            migrationBuilder.DropIndex(
                name: "IX_Picture_UserId",
                table: "Picture");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Picture");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Picture");

            migrationBuilder.AddColumn<byte[]>(
                name: "Binary",
                table: "Picture",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateTable(
                name: "PictureUser",
                columns: table => new
                {
                    PicturesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PictureUser", x => new { x.PicturesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_PictureUser_Picture_PicturesId",
                        column: x => x.PicturesId,
                        principalTable: "Picture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PictureUser_User_UsersId",
                        column: x => x.UsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PictureUser_UsersId",
                table: "PictureUser",
                column: "UsersId");
        }
    }
}
