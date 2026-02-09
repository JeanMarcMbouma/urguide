using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrGuide.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchAnalytics",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Query = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SearchedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResultsCount = table.Column<long>(type: "bigint", nullable: false),
                    TimeTakenMs = table.Column<long>(type: "bigint", nullable: false),
                    Filters = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SearchType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasResults = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchAnalytics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchAnalytics_Query",
                schema: "ug",
                table: "SearchAnalytics",
                column: "Query");

            migrationBuilder.CreateIndex(
                name: "IX_SearchAnalytics_SearchedAt",
                schema: "ug",
                table: "SearchAnalytics",
                column: "SearchedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SearchAnalytics_SearchType",
                schema: "ug",
                table: "SearchAnalytics",
                column: "SearchType");

            migrationBuilder.CreateIndex(
                name: "IX_SearchAnalytics_UserId",
                schema: "ug",
                table: "SearchAnalytics",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchAnalytics",
                schema: "ug");
        }
    }
}
