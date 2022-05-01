using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aimo.Data.Migrations
{
    public partial class added_swipe_group : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SwipeType",
                table: "SwipeHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SwipeGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AgeTo = table.Column<int>(type: "int", nullable: false),
                    AgeFrom = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    GroupType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwipeGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SwipeGroupInterest",
                columns: table => new
                {
                    SwipeGroupId = table.Column<int>(type: "int", nullable: false),
                    InterestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwipeGroupInterest", x => new { x.InterestId, x.SwipeGroupId });
                    table.ForeignKey(
                        name: "FK_SwipeGroupInterest_Interest_InterestId",
                        column: x => x.InterestId,
                        principalTable: "Interest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SwipeGroupInterest_SwipeGroup_SwipeGroupId",
                        column: x => x.SwipeGroupId,
                        principalTable: "SwipeGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SwipeGroupInterest_SwipeGroupId",
                table: "SwipeGroupInterest",
                column: "SwipeGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SwipeGroupInterest");

            migrationBuilder.DropTable(
                name: "SwipeGroup");

            migrationBuilder.DropColumn(
                name: "SwipeType",
                table: "SwipeHistory");
        }
    }
}
