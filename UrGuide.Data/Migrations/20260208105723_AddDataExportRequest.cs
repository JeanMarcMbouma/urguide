using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrGuide.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDataExportRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "authors");

            migrationBuilder.DropTable(
                name: "regions");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "tours");

            migrationBuilder.RenameTable(
                name: "Image_Catalogs_Attributes",
                newName: "Image_Catalogs_Attributes",
                newSchema: "ug");

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                schema: "ug",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "ug",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Schedule_ActiveFrom",
                schema: "ug",
                table: "tours",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Schedule_ActiveUntil",
                schema: "ug",
                table: "tours",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Schedule_EndTime",
                schema: "ug",
                table: "tours",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Schedule_NextRun",
                schema: "ug",
                table: "tours",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Schedule_StartTime",
                schema: "ug",
                table: "tours",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Schedule_Type",
                schema: "ug",
                table: "tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stats_Likes",
                schema: "ug",
                table: "tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stats_MapItsCount",
                schema: "ug",
                table: "tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stats_Rating",
                schema: "ug",
                table: "tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stats_ReactionsCount",
                schema: "ug",
                table: "tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stats_ReservedSeats",
                schema: "ug",
                table: "tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stats_ReviewsCount",
                schema: "ug",
                table: "tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stats_SharedCount",
                schema: "ug",
                table: "tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stats_Views",
                schema: "ug",
                table: "tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreditCard_CardHolderName",
                schema: "ug",
                table: "subscriptions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreditCard_CardNumber",
                schema: "ug",
                table: "subscriptions",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CreditCard_ExpiryMonth",
                schema: "ug",
                table: "subscriptions",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "CreditCard_ExpiryYear",
                schema: "ug",
                table: "subscriptions",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Flags_Active",
                schema: "ug",
                table: "regions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Flags_CanMakePayments",
                schema: "ug",
                table: "regions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Flags_CanMakeReservations",
                schema: "ug",
                table: "regions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Flags_CanRaiseTourRequests",
                schema: "ug",
                table: "regions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Flags_CanRegisterUsers",
                schema: "ug",
                table: "regions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stats_RegisteredGuides",
                schema: "ug",
                table: "regions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stats_RegisteredUsers",
                schema: "ug",
                table: "regions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stats_ToursOverallCount",
                schema: "ug",
                table: "regions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Activity_LastActive",
                schema: "ug",
                table: "authors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProfileInfo_CreatedAt",
                schema: "ug",
                table: "authors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileInfo_FirstName",
                schema: "ug",
                table: "authors",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileInfo_ImageUrl",
                schema: "ug",
                table: "authors",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileInfo_PhoneNumber",
                schema: "ug",
                table: "authors",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProfileInfo_UpdatedAt",
                schema: "ug",
                table: "authors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DataExportRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DownloadToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataExportRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataExportRequests_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                schema: "ug",
                columns: table => new
                {
                    PaymentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    BookingId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StripeCustomerId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PlatformFeeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GuidePayout = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_payments_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_payments_currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "ug",
                        principalTable: "currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_payments_tour_booking_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "ug",
                        principalTable: "tour_booking",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payouts",
                schema: "ug",
                columns: table => new
                {
                    PayoutId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GuideId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StripePayoutId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StripeAccountId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payouts", x => x.PayoutId);
                    table.ForeignKey(
                        name: "FK_payouts_authors_GuideId",
                        column: x => x.GuideId,
                        principalSchema: "ug",
                        principalTable: "authors",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tour_requests",
                schema: "ug",
                columns: table => new
                {
                    TourRequestId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    PreferredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false),
                    MaxBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequesterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_requests", x => x.TourRequestId);
                    table.ForeignKey(
                        name: "FK_tour_requests_Users_RequesterId",
                        column: x => x.RequesterId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_requests_regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "ug",
                        principalTable: "regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_transactions",
                schema: "ug",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StripeTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_payment_transactions_payments_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "ug",
                        principalTable: "payments",
                        principalColumn: "PaymentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "platform_fees",
                schema: "ug",
                columns: table => new
                {
                    FeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    MembershipTier = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_fees", x => x.FeeId);
                    table.ForeignKey(
                        name: "FK_platform_fees_payments_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "ug",
                        principalTable: "payments",
                        principalColumn: "PaymentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refunds",
                schema: "ug",
                columns: table => new
                {
                    RefundId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StripeRefundId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refunds", x => x.RefundId);
                    table.ForeignKey(
                        name: "FK_refunds_Users_RequestedBy",
                        column: x => x.RequestedBy,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_refunds_payments_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "ug",
                        principalTable: "payments",
                        principalColumn: "PaymentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                schema: "ug",
                table: "Users",
                keyColumn: "UserId",
                keyValue: "00000000-0000-0000-0000-000000000000",
                columns: new[] { "StripeCustomerId", "UserName" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_DataExportRequests_DownloadToken",
                table: "DataExportRequests",
                column: "DownloadToken");

            migrationBuilder.CreateIndex(
                name: "IX_DataExportRequests_ExpiresAt",
                table: "DataExportRequests",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_DataExportRequests_Status",
                table: "DataExportRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DataExportRequests_UserId",
                table: "DataExportRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_transactions_CreatedAt",
                schema: "ug",
                table: "payment_transactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_payment_transactions_PaymentId",
                schema: "ug",
                table: "payment_transactions",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_transactions_Type",
                schema: "ug",
                table: "payment_transactions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_payments_BookingId",
                schema: "ug",
                table: "payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_payments_CreatedAt",
                schema: "ug",
                table: "payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_payments_CurrencyCode",
                schema: "ug",
                table: "payments",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_payments_Status",
                schema: "ug",
                table: "payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_payments_StripePaymentIntentId",
                schema: "ug",
                table: "payments",
                column: "StripePaymentIntentId");

            migrationBuilder.CreateIndex(
                name: "IX_payments_UserId",
                schema: "ug",
                table: "payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_payouts_CreatedAt",
                schema: "ug",
                table: "payouts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_payouts_GuideId",
                schema: "ug",
                table: "payouts",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_payouts_RequestedAt",
                schema: "ug",
                table: "payouts",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_payouts_Status",
                schema: "ug",
                table: "payouts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_platform_fees_CreatedAt",
                schema: "ug",
                table: "platform_fees",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_platform_fees_MembershipTier",
                schema: "ug",
                table: "platform_fees",
                column: "MembershipTier");

            migrationBuilder.CreateIndex(
                name: "IX_platform_fees_PaymentId",
                schema: "ug",
                table: "platform_fees",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_refunds_CreatedAt",
                schema: "ug",
                table: "refunds",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_refunds_PaymentId",
                schema: "ug",
                table: "refunds",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_refunds_RequestedAt",
                schema: "ug",
                table: "refunds",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_refunds_RequestedBy",
                schema: "ug",
                table: "refunds",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_refunds_Status",
                schema: "ug",
                table: "refunds",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_tour_requests_CreatedAt",
                schema: "ug",
                table: "tour_requests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tour_requests_RegionId",
                schema: "ug",
                table: "tour_requests",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_tour_requests_RequesterId",
                schema: "ug",
                table: "tour_requests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_tour_requests_Status",
                schema: "ug",
                table: "tour_requests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataExportRequests");

            migrationBuilder.DropTable(
                name: "payment_transactions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "payouts",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "platform_fees",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "refunds",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tour_requests",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "ug");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                schema: "ug",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "ug",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Schedule_ActiveFrom",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Schedule_ActiveUntil",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Schedule_EndTime",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Schedule_NextRun",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Schedule_StartTime",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Schedule_Type",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Stats_Likes",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Stats_MapItsCount",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Stats_Rating",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Stats_ReactionsCount",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Stats_ReservedSeats",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Stats_ReviewsCount",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Stats_SharedCount",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "Stats_Views",
                schema: "ug",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "CreditCard_CardHolderName",
                schema: "ug",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "CreditCard_CardNumber",
                schema: "ug",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "CreditCard_ExpiryMonth",
                schema: "ug",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "CreditCard_ExpiryYear",
                schema: "ug",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "Flags_Active",
                schema: "ug",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "Flags_CanMakePayments",
                schema: "ug",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "Flags_CanMakeReservations",
                schema: "ug",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "Flags_CanRaiseTourRequests",
                schema: "ug",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "Flags_CanRegisterUsers",
                schema: "ug",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "Stats_RegisteredGuides",
                schema: "ug",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "Stats_RegisteredUsers",
                schema: "ug",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "Stats_ToursOverallCount",
                schema: "ug",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "Activity_LastActive",
                schema: "ug",
                table: "authors");

            migrationBuilder.DropColumn(
                name: "ProfileInfo_CreatedAt",
                schema: "ug",
                table: "authors");

            migrationBuilder.DropColumn(
                name: "ProfileInfo_FirstName",
                schema: "ug",
                table: "authors");

            migrationBuilder.DropColumn(
                name: "ProfileInfo_ImageUrl",
                schema: "ug",
                table: "authors");

            migrationBuilder.DropColumn(
                name: "ProfileInfo_PhoneNumber",
                schema: "ug",
                table: "authors");

            migrationBuilder.DropColumn(
                name: "ProfileInfo_UpdatedAt",
                schema: "ug",
                table: "authors");

            migrationBuilder.RenameTable(
                name: "Image_Catalogs_Attributes",
                schema: "ug",
                newName: "Image_Catalogs_Attributes");

            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastActive = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.AuthorId);
                    table.ForeignKey(
                        name: "FK_authors_authors_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "ug",
                        principalTable: "authors",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    RegionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CanMakePayments = table.Column<bool>(type: "bit", nullable: false),
                    CanMakeReservations = table.Column<bool>(type: "bit", nullable: false),
                    CanRaiseTourRequests = table.Column<bool>(type: "bit", nullable: false),
                    CanRegisterUsers = table.Column<bool>(type: "bit", nullable: false),
                    RegisteredGuides = table.Column<int>(type: "int", nullable: false),
                    RegisteredUsers = table.Column<int>(type: "int", nullable: false),
                    ToursOverallCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regions", x => x.RegionId);
                    table.ForeignKey(
                        name: "FK_regions_regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "ug",
                        principalTable: "regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CardHolderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ExpiryMonth = table.Column<byte>(type: "tinyint", nullable: false),
                    ExpiryYear = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_subscriptions_subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "ug",
                        principalTable: "subscriptions",
                        principalColumn: "SubscriptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tours",
                columns: table => new
                {
                    TourId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActiveUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    NextRun = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Likes = table.Column<int>(type: "int", nullable: false),
                    MapItsCount = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ReactionsCount = table.Column<int>(type: "int", nullable: false),
                    ReservedSeats = table.Column<int>(type: "int", nullable: false),
                    ReviewsCount = table.Column<int>(type: "int", nullable: false),
                    SharedCount = table.Column<int>(type: "int", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tours", x => x.TourId);
                    table.ForeignKey(
                        name: "FK_tours_tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "ug",
                        principalTable: "tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
