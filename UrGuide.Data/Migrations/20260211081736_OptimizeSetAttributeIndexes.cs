using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrGuide.Data.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeSetAttributeIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_User_Attributes_Name",
                schema: "ug",
                table: "User_Attributes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_File_Attributes_Name",
                schema: "ug",
                table: "File_Attributes",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Attributes_Name",
                schema: "ug",
                table: "User_Attributes");

            migrationBuilder.DropIndex(
                name: "IX_File_Attributes_Name",
                schema: "ug",
                table: "File_Attributes");
        }
    }
}
