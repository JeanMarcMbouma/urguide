using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Bid_Feedback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageCatalogId",
                schema: "ug",
                table: "Image_Catalog_Files",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "Post_Bids",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    FK_Post_Bids_Users = table.Column<string>(nullable: true),
                    NewValue = table.Column<string>(maxLength: 200, nullable: false),
                    OldValue = table.Column<string>(maxLength: 200, nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    PostId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Bids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Bids_Users_FK_Post_Bids_Users",
                        column: x => x.FK_Post_Bids_Users,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Post_Bids_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Bids_History",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    Created = table.Column<DateTime>(nullable: false),
                    FK_Post_Bids_History_Users = table.Column<string>(nullable: true),
                    Value = table.Column<string>(maxLength: 200, nullable: false),
                    PostId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Bids_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Bids_History_Users_FK_Post_Bids_History_Users",
                        column: x => x.FK_Post_Bids_History_Users,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Post_Bids_History_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Feedback",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    Text = table.Column<string>(maxLength: 2000, nullable: false),
                    FK_Post_Feedback_Users = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    PostId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Feedback_Users_FK_Post_Feedback_Users",
                        column: x => x.FK_Post_Feedback_Users,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Post_Feedback_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Feedback",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    Text = table.Column<string>(maxLength: 2000, nullable: false),
                    FK_User_Feedback_Users = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Feedback_Users_FK_User_Feedback_Users",
                        column: x => x.FK_User_Feedback_Users,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_Feedback_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_Bids_FK_Post_Bids_Users",
                schema: "ug",
                table: "Post_Bids",
                column: "FK_Post_Bids_Users");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Bids_PostId",
                schema: "ug",
                table: "Post_Bids",
                column: "PostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_Bids_History_FK_Post_Bids_History_Users",
                schema: "ug",
                table: "Post_Bids_History",
                column: "FK_Post_Bids_History_Users");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Bids_History_PostId",
                schema: "ug",
                table: "Post_Bids_History",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Feedback_FK_Post_Feedback_Users",
                schema: "ug",
                table: "Post_Feedback",
                column: "FK_Post_Feedback_Users");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Feedback_PostId",
                schema: "ug",
                table: "Post_Feedback",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Feedback_FK_User_Feedback_Users",
                schema: "ug",
                table: "User_Feedback",
                column: "FK_User_Feedback_Users");

            migrationBuilder.CreateIndex(
                name: "IX_User_Feedback_UserId",
                schema: "ug",
                table: "User_Feedback",
                column: "UserId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "Post_Bids",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_Bids_History",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_Feedback",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "User_Feedback",
                schema: "ug");

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
