using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Initial_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ug");

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "ug",
                columns: table => new
                {
                    MessageId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    To = table.Column<string>(maxLength: 200, nullable: false),
                    Subject = table.Column<string>(maxLength: 200, nullable: false),
                    Content = table.Column<string>(maxLength: 1000, nullable: false),
                    Sent = table.Column<bool>(nullable: false, defaultValue: false),
                    HasError = table.Column<bool>(nullable: false, defaultValue: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "ug",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LastActivityDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Message_Links",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    Token = table.Column<string>(maxLength: 100, nullable: false),
                    Url = table.Column<string>(maxLength: 2000, nullable: false),
                    MessageId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message_Links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_Links_Notifications_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "ug",
                        principalTable: "Notifications",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Image_Catalogs",
                schema: "ug",
                columns: table => new
                {
                    Image_CatalogId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    Created = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image_Catalogs", x => x.Image_CatalogId);
                    table.ForeignKey(
                        name: "FK_Image_Catalogs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_Attributes",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Value = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Attributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Attributes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Images",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    ImageBase64 = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Images_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Image_Catalog_Files",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    FileBase64 = table.Column<string>(nullable: false),
                    MimeType = table.Column<string>(nullable: false),
                    ImageCatalogId = table.Column<string>(nullable: true),
                    Image_CatalogId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image_Catalog_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image_Catalog_Files_Image_Catalogs_ImageCatalogId",
                        column: x => x.ImageCatalogId,
                        principalSchema: "ug",
                        principalTable: "Image_Catalogs",
                        principalColumn: "Image_CatalogId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Image_Catalog_Files_Image_Catalogs_Image_CatalogId",
                        column: x => x.Image_CatalogId,
                        principalSchema: "ug",
                        principalTable: "Image_Catalogs",
                        principalColumn: "Image_CatalogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "ug",
                columns: table => new
                {
                    PostId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    DateOfPublication = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CatalogId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_Image_Catalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalSchema: "ug",
                        principalTable: "Image_Catalogs",
                        principalColumn: "Image_CatalogId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "File_Attributes",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Value = table.Column<string>(nullable: false),
                    FileId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File_Attributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_File_Attributes_Image_Catalog_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "ug",
                        principalTable: "Image_Catalog_Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Attributes",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Value = table.Column<string>(nullable: false),
                    PostId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Attributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Attributes_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Categories",
                schema: "ug",
                columns: table => new
                {
                    CategoryId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    CategoryName = table.Column<string>(maxLength: 200, nullable: false),
                    Image_Id = table.Column<string>(nullable: true),
                    Image_ImageBase64 = table.Column<string>(nullable: true),
                    Image_MimeType = table.Column<string>(nullable: true),
                    Image_ImageCatalogId = table.Column<string>(nullable: true),
                    Archived = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    PostId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Categories", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_Post_Categories_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Post_Categories_Image_Catalogs_Image_ImageCatalogId",
                        column: x => x.Image_ImageCatalogId,
                        principalSchema: "ug",
                        principalTable: "Image_Catalogs",
                        principalColumn: "Image_CatalogId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Post_Categories_Attributes",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Value = table.Column<string>(nullable: false),
                    CategoryId = table.Column<string>(nullable: false)
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
                    ImageCategoryId = table.Column<string>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
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
                name: "IX_File_Attributes_FileId",
                schema: "ug",
                table: "File_Attributes",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_Catalog_Files_ImageCatalogId",
                schema: "ug",
                table: "Image_Catalog_Files",
                column: "ImageCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_Catalog_Files_Image_CatalogId",
                schema: "ug",
                table: "Image_Catalog_Files",
                column: "Image_CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_Catalogs_UserId",
                schema: "ug",
                table: "Image_Catalogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_Links_MessageId",
                schema: "ug",
                table: "Message_Links",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Attributes_PostId",
                schema: "ug",
                table: "Post_Attributes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Categories_PostId",
                schema: "ug",
                table: "Post_Categories",
                column: "PostId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CatalogId",
                schema: "ug",
                table: "Posts",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                schema: "ug",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Attributes_UserId",
                schema: "ug",
                table: "User_Attributes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Images_UserId",
                schema: "ug",
                table: "User_Images",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "File_Attributes",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Message_Links",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_Attributes",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_Categories_Attributes",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_Categories_Attributes1",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "User_Attributes",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "User_Images",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Image_Catalog_Files",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_Categories",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Image_Catalogs",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "ug");
        }
    }
}
