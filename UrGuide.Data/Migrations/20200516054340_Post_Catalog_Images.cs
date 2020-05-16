using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Post_Catalog_Images : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Image_Catalogs_CatalogId",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CatalogId",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CatalogId",
                schema: "ug",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "CatalogRef",
                schema: "ug",
                table: "Posts",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageCatalogId",
                schema: "ug",
                table: "Image_Catalog_Files",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CatalogRef",
                schema: "ug",
                table: "Posts",
                column: "CatalogRef",
                unique: true,
                filter: "[CatalogRef] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Image_Catalogs_CatalogRef",
                schema: "ug",
                table: "Posts",
                column: "CatalogRef",
                principalSchema: "ug",
                principalTable: "Image_Catalogs",
                principalColumn: "Image_CatalogId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Image_Catalogs_CatalogRef",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CatalogRef",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CatalogRef",
                schema: "ug",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "CatalogId",
                schema: "ug",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageCatalogId",
                schema: "ug",
                table: "Image_Catalog_Files",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CatalogId",
                schema: "ug",
                table: "Posts",
                column: "CatalogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Image_Catalogs_CatalogId",
                schema: "ug",
                table: "Posts",
                column: "CatalogId",
                principalSchema: "ug",
                principalTable: "Image_Catalogs",
                principalColumn: "Image_CatalogId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
