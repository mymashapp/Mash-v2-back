using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aimo.Data.Migrations
{
    public partial class updated_userLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region manual_highjack - it will not create Distance colunm which is not needed
            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "UserLocation",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
            
           // throw new Exception("commented all code when applied this migration so it wont create a colunm");
            #endregion
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            #region manual_highjack
            /*migrationBuilder.DropColumn(
                name: "Distance",
                table: "UserLocation");*/
            
           // throw new Exception("commented all code when applied this migration so it wont create a colunm");
            #endregion
        }
    }
}
