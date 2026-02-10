using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrGuide.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "ug",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.UpdateData(
                schema: "ug",
                table: "Users",
                keyColumn: "UserId",
                keyValue: "00000000-0000-0000-0000-000000000000",
                column: "CreatedAt",
                value: new DateTime(2020, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "ug",
                table: "Users");
        }
    }
}
