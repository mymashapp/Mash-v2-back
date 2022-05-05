using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aimo.Data.Migrations
{
    public partial class scheduled_task_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SystemType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SucceededOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IntervalInSeconds = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTask", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTask_SystemName",
                table: "ScheduleTask",
                column: "SystemName",
                unique: true,
                filter: "[SystemName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleTask");
        }
    }
}
