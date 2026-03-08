using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrGuide.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailabilityAndVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "guide_verification_submissions",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    GuideId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByAdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guide_verification_submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guide_verification_submissions_Users_GuideId",
                        column: x => x.GuideId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guide_verification_documents",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    SubmissionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileBase64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guide_verification_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guide_verification_documents_guide_verification_submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalSchema: "ug",
                        principalTable: "guide_verification_submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guide_blocked_dates",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    GuideId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guide_blocked_dates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guide_blocked_dates_Users_GuideId",
                        column: x => x.GuideId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guide_recurring_patterns",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    GuideId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PatternType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: true),
                    DayOfMonth = table.Column<int>(type: "int", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guide_recurring_patterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guide_recurring_patterns_Users_GuideId",
                        column: x => x.GuideId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_guide_verification_submissions_GuideId",
                schema: "ug",
                table: "guide_verification_submissions",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_guide_verification_submissions_Status",
                schema: "ug",
                table: "guide_verification_submissions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_guide_verification_submissions_SubmittedAt",
                schema: "ug",
                table: "guide_verification_submissions",
                column: "SubmittedAt");

            migrationBuilder.CreateIndex(
                name: "IX_guide_verification_documents_SubmissionId",
                schema: "ug",
                table: "guide_verification_documents",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_guide_blocked_dates_GuideId_Date",
                schema: "ug",
                table: "guide_blocked_dates",
                columns: new[] { "GuideId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_guide_recurring_patterns_GuideId",
                schema: "ug",
                table: "guide_recurring_patterns",
                column: "GuideId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "guide_blocked_dates", schema: "ug");
            migrationBuilder.DropTable(name: "guide_recurring_patterns", schema: "ug");
            migrationBuilder.DropTable(name: "guide_verification_documents", schema: "ug");
            migrationBuilder.DropTable(name: "guide_verification_submissions", schema: "ug");
        }
    }
}
