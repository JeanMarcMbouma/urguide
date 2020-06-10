using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Audit_Events : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audit_Events",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    EventCode = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 600, nullable: false),
                    ReferenceId = table.Column<string>(maxLength: 500, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit_Events", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audit_Events",
                schema: "ug");
        }
    }
}
