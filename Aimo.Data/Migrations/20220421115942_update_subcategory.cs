using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aimo.Data.Migrations
{
    public partial class update_subcategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "SubCategory",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "Card",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "Card",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "DisplayAddress",
                table: "Card",
                newName: "PhoneNo");

            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "SubCategory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alias",
                table: "SubCategory");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "SubCategory",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Card",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Card",
                newName: "latitude");

            migrationBuilder.RenameColumn(
                name: "PhoneNo",
                table: "Card",
                newName: "DisplayAddress");
        }
    }
}
