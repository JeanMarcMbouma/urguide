using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Post_Itineraries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "Post_Itineraries",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: false),
                    Ordinal = table.Column<byte>(type: "tinyint", nullable: false),
                    PostId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Itineraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Itineraries_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_Itineraries_PostId",
                schema: "ug",
                table: "Post_Itineraries",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropTable(
                name: "Post_Itineraries",
                schema: "ug");
        }
    }
}
