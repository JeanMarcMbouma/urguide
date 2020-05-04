using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Category_Upgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Categories_Image_Catalogs_Image_ImageCatalogId",
                schema: "ug",
                table: "Post_Categories");

            migrationBuilder.DropTable(
                name: "Post_Categories_Attributes",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_Categories_Attributes1",
                schema: "ug");

            migrationBuilder.DropIndex(
                name: "IX_Post_Categories_Image_ImageCatalogId",
                schema: "ug",
                table: "Post_Categories");

            migrationBuilder.DropColumn(
                name: "Image_Id",
                schema: "ug",
                table: "Post_Categories");

            migrationBuilder.DropColumn(
                name: "Image_ImageBase64",
                schema: "ug",
                table: "Post_Categories");

            migrationBuilder.DropColumn(
                name: "Image_ImageCatalogId",
                schema: "ug",
                table: "Post_Categories");

            migrationBuilder.DropColumn(
                name: "Image_MimeType",
                schema: "ug",
                table: "Post_Categories");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                schema: "ug",
                table: "Post_Categories",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ImageCatalogId",
                schema: "ug",
                table: "Image_Catalog_Files",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.InsertData(
                schema: "ug",
                table: "Post_Categories",
                columns: new[] { "CategoryId", "Archived", "Created", "Image", "LastUpdated", "CategoryName" },
                values: new object[,]
                {
                    { "d1442a22-adc5-4eab-a232-6ae1fe1ad4f5", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/sport.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Sport" },
                    { "62cf86ff-755d-46fd-bf8d-ca08ba353451", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/nature.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Nature" },
                    { "057e7c41-48a2-40af-83f7-86495daa66bb", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/child.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Child" },
                    { "4dc654b1-c887-4000-8e53-309f2aad0e3d", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/historical.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Historical" },
                    { "9d78cfc4-2299-445c-9c38-d6dd9d081f2b", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/amusement.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Amusement" },
                    { "3f35dba7-d527-4c70-80cb-68d25ee2b332", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/extreme.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Extreme" }
                });

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

            migrationBuilder.DeleteData(
                schema: "ug",
                table: "Post_Categories",
                keyColumn: "CategoryId",
                keyValue: "057e7c41-48a2-40af-83f7-86495daa66bb");

            migrationBuilder.DeleteData(
                schema: "ug",
                table: "Post_Categories",
                keyColumn: "CategoryId",
                keyValue: "3f35dba7-d527-4c70-80cb-68d25ee2b332");

            migrationBuilder.DeleteData(
                schema: "ug",
                table: "Post_Categories",
                keyColumn: "CategoryId",
                keyValue: "4dc654b1-c887-4000-8e53-309f2aad0e3d");

            migrationBuilder.DeleteData(
                schema: "ug",
                table: "Post_Categories",
                keyColumn: "CategoryId",
                keyValue: "62cf86ff-755d-46fd-bf8d-ca08ba353451");

            migrationBuilder.DeleteData(
                schema: "ug",
                table: "Post_Categories",
                keyColumn: "CategoryId",
                keyValue: "9d78cfc4-2299-445c-9c38-d6dd9d081f2b");

            migrationBuilder.DeleteData(
                schema: "ug",
                table: "Post_Categories",
                keyColumn: "CategoryId",
                keyValue: "d1442a22-adc5-4eab-a232-6ae1fe1ad4f5");

            migrationBuilder.DropColumn(
                name: "Image",
                schema: "ug",
                table: "Post_Categories");

            migrationBuilder.AddColumn<string>(
                name: "Image_Id",
                schema: "ug",
                table: "Post_Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image_ImageBase64",
                schema: "ug",
                table: "Post_Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image_ImageCatalogId",
                schema: "ug",
                table: "Post_Categories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image_MimeType",
                schema: "ug",
                table: "Post_Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageCatalogId",
                schema: "ug",
                table: "Image_Catalog_Files",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Post_Categories_Attributes",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Categories_Attributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Categories_Attributes_Post_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "ug",
                        principalTable: "Post_Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Categories_Attributes1",
                schema: "ug",
                columns: table => new
                {
                    ImageCategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Categories_Attributes1", x => new { x.ImageCategoryId, x.Id });
                    table.ForeignKey(
                        name: "FK_Post_Categories_Attributes1_Post_Categories_ImageCategoryId",
                        column: x => x.ImageCategoryId,
                        principalSchema: "ug",
                        principalTable: "Post_Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_Categories_Image_ImageCatalogId",
                schema: "ug",
                table: "Post_Categories",
                column: "Image_ImageCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Categories_Attributes_CategoryId",
                schema: "ug",
                table: "Post_Categories_Attributes",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Categories_Image_Catalogs_Image_ImageCatalogId",
                schema: "ug",
                table: "Post_Categories",
                column: "Image_ImageCatalogId",
                principalSchema: "ug",
                principalTable: "Image_Catalogs",
                principalColumn: "Image_CatalogId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
