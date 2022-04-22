using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aimo.Data.Migrations
{
    public partial class update_card_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateUTC",
                table: "Card",
                newName: "DateUtc");

            migrationBuilder.AlterColumn<int>(
                name: "CardType",
                table: "Card",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateUtc",
                table: "Card",
                newName: "DateUTC");

            migrationBuilder.AlterColumn<int>(
                name: "CardType",
                table: "Card",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
