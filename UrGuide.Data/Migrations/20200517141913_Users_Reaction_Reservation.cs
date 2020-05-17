using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Users_Reaction_Reservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Post_UserReactions",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    PostId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_UserReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_UserReactions_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_UserReactions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seat_Reservations",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Seats = table.Column<int>(nullable: false),
                    PostId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seat_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seat_Reservations_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Seat_Reservations_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserReactions_PostId",
                schema: "ug",
                table: "Post_UserReactions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserReactions_UserId",
                schema: "ug",
                table: "Post_UserReactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_Reservations_PostId",
                schema: "ug",
                table: "Seat_Reservations",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_Reservations_UserId",
                schema: "ug",
                table: "Seat_Reservations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Post_UserReactions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Seat_Reservations",
                schema: "ug");
        }
    }
}
