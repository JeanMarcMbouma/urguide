using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrGuide.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGuideResponseToFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuideResponse",
                schema: "ug",
                table: "User_Feedback",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuideResponse",
                schema: "ug",
                table: "Post_Feedback",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuideResponse",
                schema: "ug",
                table: "User_Feedback");

            migrationBuilder.DropColumn(
                name: "GuideResponse",
                schema: "ug",
                table: "Post_Feedback");
        }
    }
}
