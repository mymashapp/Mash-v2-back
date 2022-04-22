using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aimo.Data.Migrations
{
    public partial class subcategory_and_card_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Card_Category_CategoryId",
                table: "Card");

            migrationBuilder.RenameColumn(
                name: "Zip",
                table: "Card",
                newName: "ZipCode");

            migrationBuilder.RenameColumn(
                name: "EventName",
                table: "Card",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Card",
                newName: "State");

            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "Card",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "Card",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address3",
                table: "Card",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "Card",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Card",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Card",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayAddress",
                table: "Card",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Card",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Phone",
                table: "Card",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "Card",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "Card",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SubCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CardSubCategory",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardSubCategory", x => new { x.CardId, x.SubCategoryId });
                    table.ForeignKey(
                        name: "FK_CardSubCategory_Card_CardId",
                        column: x => x.CardId,
                        principalTable: "Card",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardSubCategory_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardSubCategory_SubCategoryId",
                table: "CardSubCategory",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryId",
                table: "SubCategory",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Card_Category_CategoryId",
                table: "Card",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Card_Category_CategoryId",
                table: "Card");

            migrationBuilder.DropTable(
                name: "CardSubCategory");

            migrationBuilder.DropTable(
                name: "SubCategory");

            migrationBuilder.DropColumn(
                name: "Address1",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Address3",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Alias",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "DisplayAddress",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "Card");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "Card",
                newName: "Zip");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Card",
                newName: "EventName");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Card",
                newName: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_Card_Category_CategoryId",
                table: "Card",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
