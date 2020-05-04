using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Removed_Catagory_From_Post : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Categories_Posts_PostId",
                schema: "ug",
                table: "Post_Categories");

            migrationBuilder.DropIndex(
                name: "IX_Post_Categories_PostId",
                schema: "ug",
                table: "Post_Categories");

            migrationBuilder.DropColumn(
                name: "PostId",
                schema: "ug",
                table: "Post_Categories");

            migrationBuilder.AlterColumn<string>(
                name: "ImageCatalogId",
                schema: "ug",
                table: "Image_Catalog_Files",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "Image_Catalogs_Attributes",
                schema: "ug",
                columns: table => new
                {
                    ImageCatalogId = table.Column<string>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image_Catalogs_Attributes", x => new { x.ImageCatalogId, x.Id });
                    table.ForeignKey(
                        name: "FK_Image_Catalogs_Attributes_Image_Catalogs_ImageCatalogId",
                        column: x => x.ImageCatalogId,
                        principalSchema: "ug",
                        principalTable: "Image_Catalogs",
                        principalColumn: "Image_CatalogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Catalog_Files_Image_Catalogs_ImageCatalogId_1",
                schema: "ug",
                table: "Image_Catalog_Files",
                column: "ImageCatalogId",
                principalSchema: "ug",
                principalTable: "Image_Catalogs",
                principalColumn: "Image_CatalogId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Catalog_Files_Image_Catalogs_ImageCatalogId_1",
                schema: "ug",
                table: "Image_Catalog_Files");

            migrationBuilder.DropTable(
                name: "Image_Catalogs_Attributes",
                schema: "ug");

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                schema: "ug",
                table: "Post_Categories",
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
                name: "IX_Post_Categories_PostId",
                schema: "ug",
                table: "Post_Categories",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Categories_Posts_PostId",
                schema: "ug",
                table: "Post_Categories",
                column: "PostId",
                principalSchema: "ug",
                principalTable: "Posts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
