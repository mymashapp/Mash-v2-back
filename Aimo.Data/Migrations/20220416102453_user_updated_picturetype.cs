using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aimo.Data.Migrations
{
    public partial class user_updated_picturetype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureId",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "PictureType",
                table: "Picture",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureType",
                table: "Picture");

            migrationBuilder.AddColumn<int>(
                name: "ProfilePictureId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
