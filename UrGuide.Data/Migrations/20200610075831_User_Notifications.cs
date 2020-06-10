using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class User_Notifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Message_Links",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "ug");

            migrationBuilder.CreateTable(
                name: "User_Notifications",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    FK_User_Notification_Users = table.Column<string>(nullable: true),
                    Content = table.Column<string>(maxLength: 500, nullable: false),
                    ReferenceLink = table.Column<string>(maxLength: 1000, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Read = table.Column<bool>(nullable: false),
                    IsSystem = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Notifications_Users_FK_User_Notification_Users",
                        column: x => x.FK_User_Notification_Users,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "ug",
                table: "Users",
                columns: new[] { "UserId", "LastActivityDate", "Location" },
                values: new object[] { "00000000-0000-0000-0000-000000000000", new DateTime(2020, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.CreateIndex(
                name: "IX_User_Notifications_FK_User_Notification_Users",
                schema: "ug",
                table: "User_Notifications",
                column: "FK_User_Notification_Users");

            migrationBuilder.CreateIndex(
                name: "IX_User_Notifications_UserId",
                schema: "ug",
                table: "User_Notifications",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User_Notifications",
                schema: "ug");

            migrationBuilder.DeleteData(
                schema: "ug",
                table: "Users",
                keyColumn: "UserId",
                keyValue: "00000000-0000-0000-0000-000000000000");

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "ug",
                columns: table => new
                {
                    MessageId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasError = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Sent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    To = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                });

            migrationBuilder.CreateTable(
                name: "Message_Links",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    MessageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Message_Links_MessageId",
                schema: "ug",
                table: "Message_Links",
                column: "MessageId");
        }
    }
}
