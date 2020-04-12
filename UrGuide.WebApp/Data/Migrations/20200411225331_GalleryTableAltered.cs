using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.WebApp.Data.Migrations
{
    public partial class GalleryTableAltered : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Shots_Table");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Shots_Table",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Shots_Table",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Galleries_Table",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Galleries_Table",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Shots_Table");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Shots_Table");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Galleries_Table");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Galleries_Table");

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "Shots_Table",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
