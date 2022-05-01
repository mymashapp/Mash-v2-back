using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aimo.Data.Migrations
{
    public partial class user_preference_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreference");

            migrationBuilder.AddColumn<int>(
                name: "PreferenceAgeFrom",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreferenceAgeTo",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PreferenceGender",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreferenceGroupOf",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferenceAgeFrom",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PreferenceAgeTo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PreferenceGender",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PreferenceGroupOf",
                table: "User");

            migrationBuilder.CreateTable(
                name: "UserPreference",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgeFrom = table.Column<int>(type: "int", nullable: false),
                    AgeTo = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupOf = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreference_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreference_UserId",
                table: "UserPreference",
                column: "UserId",
                unique: true);
        }
    }
}
