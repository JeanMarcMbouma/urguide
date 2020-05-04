using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace UrGuide.Data.Migrations
{
    public partial class Spacial_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "Location",
                schema: "ug",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                schema: "ug",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                schema: "ug",
                table: "Image_Catalogs",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageCatalogId",
                schema: "ug",
                table: "Image_Catalog_Files",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Image_Catalog_Files_Image_Catalogs_ImageCatalogId",
            //    schema: "ug",
            //    table: "Image_Catalog_Files",
            //    column: "ImageCatalogId",
            //    principalSchema: "ug",
            //    principalTable: "Image_Catalogs",
            //    principalColumn: "Image_CatalogId",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Image_Catalog_Files_Image_Catalogs_ImageCatalogId",
            //    schema: "ug",
            //    table: "Image_Catalog_Files");

            migrationBuilder.DropColumn(
                name: "Location",
                schema: "ug",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Location",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Location",
                schema: "ug",
                table: "Image_Catalogs");

            migrationBuilder.AlterColumn<string>(
                name: "ImageCatalogId",
                schema: "ug",
                table: "Image_Catalog_Files",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
