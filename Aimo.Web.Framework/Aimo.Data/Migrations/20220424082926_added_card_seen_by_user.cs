using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aimo.Data.Migrations
{
    public partial class added_card_seen_by_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardSeenByUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SeenAtUct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardSeenByUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardSeenByUser_Card_CardId",
                        column: x => x.CardId,
                        principalTable: "Card",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardSeenByUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardSeenByUser_CardId",
                table: "CardSeenByUser",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardSeenByUser_UserId",
                table: "CardSeenByUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardSeenByUser");
        }
    }
}
