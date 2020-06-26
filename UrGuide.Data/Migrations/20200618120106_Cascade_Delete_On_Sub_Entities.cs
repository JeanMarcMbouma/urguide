using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Cascade_Delete_On_Sub_Entities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Bids_Users_FK_Post_Bids_Users",
                schema: "ug",
                table: "Post_Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Bids_History_Users_FK_Post_Bids_History_Users",
                schema: "ug",
                table: "Post_Bids_History");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Feedback_Users_FK_Post_Feedback_Users",
                schema: "ug",
                table: "Post_Feedback");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Bids_Users_FK_Post_Bids_Users",
                schema: "ug",
                table: "Post_Bids",
                column: "FK_Post_Bids_Users",
                principalSchema: "ug",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Bids_History_Users_FK_Post_Bids_History_Users",
                schema: "ug",
                table: "Post_Bids_History",
                column: "FK_Post_Bids_History_Users",
                principalSchema: "ug",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Feedback_Users_FK_Post_Feedback_Users",
                schema: "ug",
                table: "Post_Feedback",
                column: "FK_Post_Feedback_Users",
                principalSchema: "ug",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Bids_Users_FK_Post_Bids_Users",
                schema: "ug",
                table: "Post_Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Bids_History_Users_FK_Post_Bids_History_Users",
                schema: "ug",
                table: "Post_Bids_History");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Feedback_Users_FK_Post_Feedback_Users",
                schema: "ug",
                table: "Post_Feedback");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Bids_Users_FK_Post_Bids_Users",
                schema: "ug",
                table: "Post_Bids",
                column: "FK_Post_Bids_Users",
                principalSchema: "ug",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Bids_History_Users_FK_Post_Bids_History_Users",
                schema: "ug",
                table: "Post_Bids_History",
                column: "FK_Post_Bids_History_Users",
                principalSchema: "ug",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Feedback_Users_FK_Post_Feedback_Users",
                schema: "ug",
                table: "Post_Feedback",
                column: "FK_Post_Feedback_Users",
                principalSchema: "ug",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
