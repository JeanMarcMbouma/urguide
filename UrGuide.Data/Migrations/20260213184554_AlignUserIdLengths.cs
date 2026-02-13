using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrGuide.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlignUserIdLengths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "User_Notifications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)");

            migrationBuilder.AlterColumn<string>(
                name: "FK_User_Notification_Users",
                schema: "ug",
                table: "User_Notifications",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "User_Images",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "User_Feedback",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)");

            migrationBuilder.AlterColumn<string>(
                name: "FK_User_Feedback_Users",
                schema: "ug",
                table: "User_Feedback",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "User_Attributes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)");

            migrationBuilder.AlterColumn<string>(
                name: "RequesterId",
                schema: "ug",
                table: "tour_requests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                schema: "ug",
                table: "tour_booking",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "Seat_Reservations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "Post_UserReactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)");

            migrationBuilder.AlterColumn<string>(
                name: "FK_Post_Feedback_Users",
                schema: "ug",
                table: "Post_Feedback",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FK_Post_Bids_History_Users",
                schema: "ug",
                table: "Post_Bids_History",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FK_Post_Bids_Users",
                schema: "ug",
                table: "Post_Bids",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "Image_Catalogs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "User_Notifications",
                type: "nvarchar(128)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FK_User_Notification_Users",
                schema: "ug",
                table: "User_Notifications",
                type: "nvarchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "User_Images",
                type: "nvarchar(128)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "User_Feedback",
                type: "nvarchar(128)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FK_User_Feedback_Users",
                schema: "ug",
                table: "User_Feedback",
                type: "nvarchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "User_Attributes",
                type: "nvarchar(128)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "RequesterId",
                schema: "ug",
                table: "tour_requests",
                type: "nvarchar(128)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                schema: "ug",
                table: "tour_booking",
                type: "nvarchar(128)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "Seat_Reservations",
                type: "nvarchar(128)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "Posts",
                type: "nvarchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "Post_UserReactions",
                type: "nvarchar(128)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FK_Post_Feedback_Users",
                schema: "ug",
                table: "Post_Feedback",
                type: "nvarchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FK_Post_Bids_History_Users",
                schema: "ug",
                table: "Post_Bids_History",
                type: "nvarchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FK_Post_Bids_Users",
                schema: "ug",
                table: "Post_Bids",
                type: "nvarchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "ug",
                table: "Image_Catalogs",
                type: "nvarchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
