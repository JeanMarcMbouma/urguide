using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Users_First_Last_Names : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "ug",
                table: "Users",
                maxLength: 255,
                nullable: false,
                defaultValue: "N/A");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "ug",
                table: "Users",
                maxLength: 200,
                nullable: false,
                defaultValue: "N/A");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "ug",
                table: "Users",
                maxLength: 200,
                nullable: false,
                defaultValue: "N/A");

            migrationBuilder.Sql(@"
            UPDATE u 
                SET u.FirstName = a1.Value,
	                u.LastName = a2.Value,
	                u.Email = a3.Value
            FROM ug.Users as u
            JOIN ug.User_Attributes a1 ON a1.UserId = u.UserId AND a1.Name = 'FirstName' 
            JOIN ug.User_Attributes a2 ON a2.UserId = u.UserId AND a2.Name = 'LastName' 
            JOIN ug.User_Attributes a3 ON a3.UserId = u.UserId AND a3.Name LIKE 'EmailAddress'
            ");
            migrationBuilder.Sql("DELETE FROM ug.User_Attributes WHERE Name IN ('FirstName', 'LastName', 'EmailAddress')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "ug",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "ug",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "ug",
                table: "Users");
        }
    }
}
