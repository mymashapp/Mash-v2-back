using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aimo.Data.Migrations
{
    public partial class category_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Category");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Category",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
