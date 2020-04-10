using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.WebApp.Data.Migrations
{
    public partial class PostGalleryShot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Galleries_Table",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries_Table", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts_Table",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts_Table", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shots_Table",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Photo = table.Column<string>(nullable: true),
                    HasPost = table.Column<bool>(nullable: false),
                    GalleryId = table.Column<long>(nullable: false),
                    PostId = table.Column<long>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shots_Table", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Galleries_Table");

            migrationBuilder.DropTable(
                name: "Posts_Table");

            migrationBuilder.DropTable(
                name: "Shots_Table");
        }
    }
}
