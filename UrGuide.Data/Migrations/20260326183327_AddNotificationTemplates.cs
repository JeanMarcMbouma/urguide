using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrGuide.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuideResponse",
                schema: "ug",
                table: "User_Feedback",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSpam",
                schema: "ug",
                table: "reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ModerationStatus",
                schema: "ug",
                table: "reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SpamScore",
                schema: "ug",
                table: "reviews",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GuideResponse",
                schema: "ug",
                table: "Post_Feedback",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "achievements",
                schema: "ug",
                columns: table => new
                {
                    AchievementId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ThresholdValue = table.Column<int>(type: "int", nullable: false),
                    PointsReward = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements", x => x.AchievementId);
                });

            migrationBuilder.CreateTable(
                name: "advertisements",
                schema: "ug",
                columns: table => new
                {
                    AdvertisementId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    AdvertiserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TargetUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TargetAudience = table.Column<int>(type: "int", nullable: false),
                    TargetRegionId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SpentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    Impressions = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Clicks = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_advertisements", x => x.AdvertisementId);
                });

            migrationBuilder.CreateTable(
                name: "badges",
                schema: "ug",
                columns: table => new
                {
                    BadgeId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Criteria = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ThresholdValue = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badges", x => x.BadgeId);
                });

            migrationBuilder.CreateTable(
                name: "coin_wallets",
                schema: "ug",
                columns: table => new
                {
                    CoinWalletId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    TotalEarned = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    TotalSpent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coin_wallets", x => x.CoinWalletId);
                });

            migrationBuilder.CreateTable(
                name: "conversations",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    Participant1Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Participant1Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Participant2Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Participant2Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastMessageAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "device_registrations",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DeviceToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    AppVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_device_registrations_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "disputes",
                schema: "ug",
                columns: table => new
                {
                    DisputeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BookingId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FiledBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    AgainstUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disputes", x => x.DisputeId);
                });

            migrationBuilder.CreateTable(
                name: "email_templates",
                schema: "ug",
                columns: table => new
                {
                    TemplateId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HtmlBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlainTextBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    VariablesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_templates", x => x.TemplateId);
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
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guide_google_calendar_tokens",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    GuideId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EncryptedAccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedRefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Bearer"),
                    Scope = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guide_google_calendar_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guide_google_calendar_tokens_Users_GuideId",
                        column: x => x.GuideId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
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
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lottery_draws",
                schema: "ug",
                columns: table => new
                {
                    LotteryDrawId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    TourId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MaxEntries = table.Column<int>(type: "int", nullable: false),
                    WinnerCount = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EntryDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DrawDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lottery_draws", x => x.LotteryDrawId);
                });

            migrationBuilder.CreateTable(
                name: "loyalty_accounts",
                schema: "ug",
                columns: table => new
                {
                    LoyaltyAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    DiscountPercentage = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalToursCompleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalSpent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loyalty_accounts", x => x.LoyaltyAccountId);
                });

            migrationBuilder.CreateTable(
                name: "notification_preferences",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PushEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TourUpdatesEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BookingAlertsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ChatMessagesEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PromotionalEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SystemAlertsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_preferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notification_preferences_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notification_templates",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    TitleTemplate = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BodyTemplate = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ActionUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    VariantGroup = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payout_schedules",
                schema: "ug",
                columns: table => new
                {
                    PayoutScheduleId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    GuideId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    MinimumAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NextPayoutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastPayoutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payout_schedules", x => x.PayoutScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "processed_images",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, defaultValueSql: "NEWID()"),
                    OriginalImageId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    OriginalUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MediumUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LargeUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WebPUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Format = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OriginalSize = table.Column<long>(type: "bigint", nullable: false),
                    CompressedSize = table.Column<long>(type: "bigint", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    CdnUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsWatermarked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExifDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processed_images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "recommendation_logs",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TourId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    Algorithm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WasClicked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    WasBooked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recommendation_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "referral_codes",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TotalReferrals = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalEarnings = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referral_codes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "report_definitions",
                schema: "ug",
                columns: table => new
                {
                    ReportId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    ParametersJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report_definitions", x => x.ReportId);
                });

            migrationBuilder.CreateTable(
                name: "review_flags",
                schema: "ug",
                columns: table => new
                {
                    ReviewFlagId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    ReviewId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FlaggedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review_flags", x => x.ReviewFlagId);
                    table.ForeignKey(
                        name: "FK_review_flags_Users_FlaggedBy",
                        column: x => x.FlaggedBy,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_review_flags_reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "ug",
                        principalTable: "reviews",
                        principalColumn: "ReviewId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "review_moderation_actions",
                schema: "ug",
                columns: table => new
                {
                    ActionId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    ReviewId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    PreviousContent = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review_moderation_actions", x => x.ActionId);
                    table.ForeignKey(
                        name: "FK_review_moderation_actions_Users_PerformedBy",
                        column: x => x.PerformedBy,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_review_moderation_actions_reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "ug",
                        principalTable: "reviews",
                        principalColumn: "ReviewId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scheduled_reports",
                schema: "ug",
                columns: table => new
                {
                    ScheduleId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReportType = table.Column<int>(type: "int", nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    ParametersJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    EmailRecipients = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduled_reports", x => x.ScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plans",
                schema: "ug",
                columns: table => new
                {
                    SubscriptionPlanId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    BillingCycle = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlatformFeePercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SearchRankingBoost = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxGroupSize = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_plans", x => x.SubscriptionPlanId);
                });

            migrationBuilder.CreateTable(
                name: "tour_interactions",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TourId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_interactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tour_templates",
                schema: "ug",
                columns: table => new
                {
                    TemplateId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    GuideId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DefaultDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    DefaultMaxParticipants = table.Column<int>(type: "int", nullable: false),
                    DefaultMeetingPoint = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ItineraryJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncludedItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcludedItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_templates", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "user_preferences",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PreferenceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PreferenceValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 1.0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_preferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "visibility_boosts",
                schema: "ug",
                columns: table => new
                {
                    VisibilityBoostId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    GuideId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    TourId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    BoostType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BoostMultiplier = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visibility_boosts", x => x.VisibilityBoostId);
                });

            migrationBuilder.CreateTable(
                name: "withdrawal_requests",
                schema: "ug",
                columns: table => new
                {
                    WithdrawalRequestId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoutingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountHolderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_withdrawal_requests", x => x.WithdrawalRequestId);
                });

            migrationBuilder.CreateTable(
                name: "user_achievements",
                schema: "ug",
                columns: table => new
                {
                    UserAchievementId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    AchievementId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Progress = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_achievements", x => x.UserAchievementId);
                    table.ForeignKey(
                        name: "FK_user_achievements_achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalSchema: "ug",
                        principalTable: "achievements",
                        principalColumn: "AchievementId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_badges",
                schema: "ug",
                columns: table => new
                {
                    UserBadgeId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    BadgeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EarnedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_badges", x => x.UserBadgeId);
                    table.ForeignKey(
                        name: "FK_user_badges_badges_BadgeId",
                        column: x => x.BadgeId,
                        principalSchema: "ug",
                        principalTable: "badges",
                        principalColumn: "BadgeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "coin_transactions",
                schema: "ug",
                columns: table => new
                {
                    CoinTransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    CoinWalletId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coin_transactions", x => x.CoinTransactionId);
                    table.ForeignKey(
                        name: "FK_coin_transactions_coin_wallets_CoinWalletId",
                        column: x => x.CoinWalletId,
                        principalSchema: "ug",
                        principalTable: "coin_wallets",
                        principalColumn: "CoinWalletId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    ConversationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_messages_conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "ug",
                        principalTable: "conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "push_notification_logs",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DeviceRegistrationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TemplateId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_push_notification_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_push_notification_logs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_push_notification_logs_device_registrations_DeviceRegistrationId",
                        column: x => x.DeviceRegistrationId,
                        principalSchema: "ug",
                        principalTable: "device_registrations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "dispute_evidence",
                schema: "ug",
                columns: table => new
                {
                    EvidenceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisputeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubmittedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispute_evidence", x => x.EvidenceId);
                    table.ForeignKey(
                        name: "FK_dispute_evidence_disputes_DisputeId",
                        column: x => x.DisputeId,
                        principalSchema: "ug",
                        principalTable: "disputes",
                        principalColumn: "DisputeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dispute_messages",
                schema: "ug",
                columns: table => new
                {
                    MessageId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisputeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IsAdminMessage = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispute_messages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_dispute_messages_disputes_DisputeId",
                        column: x => x.DisputeId,
                        principalSchema: "ug",
                        principalTable: "disputes",
                        principalColumn: "DisputeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "email_template_versions",
                schema: "ug",
                columns: table => new
                {
                    VersionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    TemplateId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HtmlBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlainTextBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_template_versions", x => x.VersionId);
                    table.ForeignKey(
                        name: "FK_email_template_versions_email_templates_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "ug",
                        principalTable: "email_templates",
                        principalColumn: "TemplateId",
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
                name: "lottery_entries",
                schema: "ug",
                columns: table => new
                {
                    LotteryEntryId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    LotteryDrawId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IsWinner = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EnteredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lottery_entries", x => x.LotteryEntryId);
                    table.ForeignKey(
                        name: "FK_lottery_entries_lottery_draws_LotteryDrawId",
                        column: x => x.LotteryDrawId,
                        principalSchema: "ug",
                        principalTable: "lottery_draws",
                        principalColumn: "LotteryDrawId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "loyalty_transactions",
                schema: "ug",
                columns: table => new
                {
                    LoyaltyTransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    LoyaltyAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loyalty_transactions", x => x.LoyaltyTransactionId);
                    table.ForeignKey(
                        name: "FK_loyalty_transactions_loyalty_accounts_LoyaltyAccountId",
                        column: x => x.LoyaltyAccountId,
                        principalSchema: "ug",
                        principalTable: "loyalty_accounts",
                        principalColumn: "LoyaltyAccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "referrals",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    ReferralCodeId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ReferrerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ReferredUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RewardAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RewardedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referrals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_referrals_referral_codes_ReferralCodeId",
                        column: x => x.ReferralCodeId,
                        principalSchema: "ug",
                        principalTable: "referral_codes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "guide_subscriptions",
                schema: "ug",
                columns: table => new
                {
                    GuideSubscriptionId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    GuideId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    SubscriptionPlanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    StripeSubscriptionId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guide_subscriptions", x => x.GuideSubscriptionId);
                    table.ForeignKey(
                        name: "FK_guide_subscriptions_subscription_plans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalSchema: "ug",
                        principalTable: "subscription_plans",
                        principalColumn: "SubscriptionPlanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "file_attachments",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "NEWID()"),
                    MessageId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_file_attachments_messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "ug",
                        principalTable: "messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_advertisements_AdvertiserId",
                schema: "ug",
                table: "advertisements",
                column: "AdvertiserId");

            migrationBuilder.CreateIndex(
                name: "IX_advertisements_Status",
                schema: "ug",
                table: "advertisements",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_coin_transactions_CoinWalletId",
                schema: "ug",
                table: "coin_transactions",
                column: "CoinWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_coin_transactions_CreatedAt",
                schema: "ug",
                table: "coin_transactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_coin_wallets_UserId",
                schema: "ug",
                table: "coin_wallets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_conversations_Participant1Id",
                schema: "ug",
                table: "conversations",
                column: "Participant1Id");

            migrationBuilder.CreateIndex(
                name: "IX_conversations_Participant2Id",
                schema: "ug",
                table: "conversations",
                column: "Participant2Id");

            migrationBuilder.CreateIndex(
                name: "IX_device_registrations_IsActive",
                schema: "ug",
                table: "device_registrations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_device_registrations_UserId",
                schema: "ug",
                table: "device_registrations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_device_registrations_UserId_DeviceToken",
                schema: "ug",
                table: "device_registrations",
                columns: new[] { "UserId", "DeviceToken" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dispute_evidence_DisputeId",
                schema: "ug",
                table: "dispute_evidence",
                column: "DisputeId");

            migrationBuilder.CreateIndex(
                name: "IX_dispute_evidence_SubmittedBy",
                schema: "ug",
                table: "dispute_evidence",
                column: "SubmittedBy");

            migrationBuilder.CreateIndex(
                name: "IX_dispute_messages_DisputeId",
                schema: "ug",
                table: "dispute_messages",
                column: "DisputeId");

            migrationBuilder.CreateIndex(
                name: "IX_dispute_messages_SenderId",
                schema: "ug",
                table: "dispute_messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_dispute_messages_SentAt",
                schema: "ug",
                table: "dispute_messages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_disputes_AgainstUserId",
                schema: "ug",
                table: "disputes",
                column: "AgainstUserId");

            migrationBuilder.CreateIndex(
                name: "IX_disputes_BookingId",
                schema: "ug",
                table: "disputes",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_disputes_CreatedAt",
                schema: "ug",
                table: "disputes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_disputes_FiledBy",
                schema: "ug",
                table: "disputes",
                column: "FiledBy");

            migrationBuilder.CreateIndex(
                name: "IX_disputes_Priority",
                schema: "ug",
                table: "disputes",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_disputes_Status",
                schema: "ug",
                table: "disputes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_email_template_versions_TemplateId",
                schema: "ug",
                table: "email_template_versions",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_email_template_versions_TemplateId_VersionNumber",
                schema: "ug",
                table: "email_template_versions",
                columns: new[] { "TemplateId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_email_templates_Category",
                schema: "ug",
                table: "email_templates",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_email_templates_IsActive",
                schema: "ug",
                table: "email_templates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_email_templates_Language",
                schema: "ug",
                table: "email_templates",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_email_templates_Name",
                schema: "ug",
                table: "email_templates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_email_templates_Name_Language",
                schema: "ug",
                table: "email_templates",
                columns: new[] { "Name", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_MessageId",
                schema: "ug",
                table: "file_attachments",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_guide_blocked_dates_GuideId_Date",
                schema: "ug",
                table: "guide_blocked_dates",
                columns: new[] { "GuideId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_guide_google_calendar_tokens_GuideId",
                schema: "ug",
                table: "guide_google_calendar_tokens",
                column: "GuideId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_guide_recurring_patterns_GuideId",
                schema: "ug",
                table: "guide_recurring_patterns",
                column: "GuideId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_guide_subscriptions_GuideId",
                schema: "ug",
                table: "guide_subscriptions",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_guide_subscriptions_Status",
                schema: "ug",
                table: "guide_subscriptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_guide_subscriptions_SubscriptionPlanId",
                schema: "ug",
                table: "guide_subscriptions",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_guide_verification_documents_SubmissionId",
                schema: "ug",
                table: "guide_verification_documents",
                column: "SubmissionId");

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
                name: "IX_lottery_draws_Status",
                schema: "ug",
                table: "lottery_draws",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_lottery_entries_LotteryDrawId_UserId",
                schema: "ug",
                table: "lottery_entries",
                columns: new[] { "LotteryDrawId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_accounts_UserId",
                schema: "ug",
                table: "loyalty_accounts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_transactions_CreatedAt",
                schema: "ug",
                table: "loyalty_transactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_transactions_LoyaltyAccountId",
                schema: "ug",
                table: "loyalty_transactions",
                column: "LoyaltyAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_ConversationId",
                schema: "ug",
                table: "messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_SenderId",
                schema: "ug",
                table: "messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_notification_preferences_UserId",
                schema: "ug",
                table: "notification_preferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_templates_Category",
                schema: "ug",
                table: "notification_templates",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_notification_templates_Name_Language_IsActive",
                schema: "ug",
                table: "notification_templates",
                columns: new[] { "Name", "Language", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_notification_templates_Name_Language_Version",
                schema: "ug",
                table: "notification_templates",
                columns: new[] { "Name", "Language", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_payout_schedules_GuideId",
                schema: "ug",
                table: "payout_schedules",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_payout_schedules_Status",
                schema: "ug",
                table: "payout_schedules",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_processed_images_CreatedAt",
                schema: "ug",
                table: "processed_images",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_processed_images_OriginalImageId",
                schema: "ug",
                table: "processed_images",
                column: "OriginalImageId");

            migrationBuilder.CreateIndex(
                name: "IX_processed_images_Status",
                schema: "ug",
                table: "processed_images",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_push_notification_logs_DeviceRegistrationId",
                schema: "ug",
                table: "push_notification_logs",
                column: "DeviceRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_push_notification_logs_SentAt",
                schema: "ug",
                table: "push_notification_logs",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_push_notification_logs_Status",
                schema: "ug",
                table: "push_notification_logs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_push_notification_logs_UserId",
                schema: "ug",
                table: "push_notification_logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_logs_Algorithm",
                schema: "ug",
                table: "recommendation_logs",
                column: "Algorithm");

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_logs_CreatedAt",
                schema: "ug",
                table: "recommendation_logs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_logs_TourId",
                schema: "ug",
                table: "recommendation_logs",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_logs_UserId",
                schema: "ug",
                table: "recommendation_logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_referral_codes_Code",
                schema: "ug",
                table: "referral_codes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_referral_codes_IsActive",
                schema: "ug",
                table: "referral_codes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_referral_codes_UserId",
                schema: "ug",
                table: "referral_codes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_referrals_ReferralCodeId",
                schema: "ug",
                table: "referrals",
                column: "ReferralCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_referrals_ReferredUserId",
                schema: "ug",
                table: "referrals",
                column: "ReferredUserId");

            migrationBuilder.CreateIndex(
                name: "IX_referrals_ReferrerId",
                schema: "ug",
                table: "referrals",
                column: "ReferrerId");

            migrationBuilder.CreateIndex(
                name: "IX_referrals_Status",
                schema: "ug",
                table: "referrals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_report_definitions_CreatedAt",
                schema: "ug",
                table: "report_definitions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_report_definitions_RequestedBy",
                schema: "ug",
                table: "report_definitions",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_report_definitions_Status",
                schema: "ug",
                table: "report_definitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_review_flags_FlaggedBy",
                schema: "ug",
                table: "review_flags",
                column: "FlaggedBy");

            migrationBuilder.CreateIndex(
                name: "IX_review_flags_ReviewId",
                schema: "ug",
                table: "review_flags",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_review_moderation_actions_PerformedBy",
                schema: "ug",
                table: "review_moderation_actions",
                column: "PerformedBy");

            migrationBuilder.CreateIndex(
                name: "IX_review_moderation_actions_ReviewId",
                schema: "ug",
                table: "review_moderation_actions",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_scheduled_reports_IsActive",
                schema: "ug",
                table: "scheduled_reports",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_scheduled_reports_NextRunAt",
                schema: "ug",
                table: "scheduled_reports",
                column: "NextRunAt");

            migrationBuilder.CreateIndex(
                name: "IX_scheduled_reports_UserId",
                schema: "ug",
                table: "scheduled_reports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tour_interactions_TourId",
                schema: "ug",
                table: "tour_interactions",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_tour_interactions_UserId",
                schema: "ug",
                table: "tour_interactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tour_interactions_UserId_TourId_Type",
                schema: "ug",
                table: "tour_interactions",
                columns: new[] { "UserId", "TourId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_user_achievements_AchievementId",
                schema: "ug",
                table: "user_achievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_user_achievements_UserId_AchievementId",
                schema: "ug",
                table: "user_achievements",
                columns: new[] { "UserId", "AchievementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_badges_BadgeId",
                schema: "ug",
                table: "user_badges",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_user_badges_UserId_BadgeId",
                schema: "ug",
                table: "user_badges",
                columns: new[] { "UserId", "BadgeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_preferences_UserId",
                schema: "ug",
                table: "user_preferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_preferences_UserId_PreferenceType",
                schema: "ug",
                table: "user_preferences",
                columns: new[] { "UserId", "PreferenceType" });

            migrationBuilder.CreateIndex(
                name: "IX_visibility_boosts_GuideId",
                schema: "ug",
                table: "visibility_boosts",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_visibility_boosts_Status",
                schema: "ug",
                table: "visibility_boosts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_requests_Status",
                schema: "ug",
                table: "withdrawal_requests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_requests_UserId",
                schema: "ug",
                table: "withdrawal_requests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "advertisements",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "coin_transactions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "dispute_evidence",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "dispute_messages",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "email_template_versions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "file_attachments",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "guide_blocked_dates",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "guide_google_calendar_tokens",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "guide_recurring_patterns",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "guide_subscriptions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "guide_verification_documents",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "lottery_entries",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "loyalty_transactions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "notification_preferences",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "notification_templates",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "payout_schedules",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "processed_images",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "push_notification_logs",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "recommendation_logs",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "referrals",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "report_definitions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "review_flags",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "review_moderation_actions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "scheduled_reports",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tour_interactions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tour_templates",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "user_achievements",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "user_badges",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "user_preferences",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "visibility_boosts",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "withdrawal_requests",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "coin_wallets",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "disputes",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "email_templates",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "messages",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "subscription_plans",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "guide_verification_submissions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "lottery_draws",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "loyalty_accounts",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "device_registrations",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "referral_codes",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "achievements",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "badges",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "conversations",
                schema: "ug");

            migrationBuilder.DropColumn(
                name: "GuideResponse",
                schema: "ug",
                table: "User_Feedback");

            migrationBuilder.DropColumn(
                name: "IsSpam",
                schema: "ug",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "ModerationStatus",
                schema: "ug",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "SpamScore",
                schema: "ug",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "GuideResponse",
                schema: "ug",
                table: "Post_Feedback");
        }
    }
}
