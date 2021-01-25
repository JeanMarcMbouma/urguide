using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class new_author_tour_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "countries",
                schema: "ug",
                columns: table => new
                {
                    CountryId = table.Column<string>(maxLength: 200, nullable: false),
                    Code = table.Column<string>(maxLength: 10, nullable: false),
                    DialCode = table.Column<string>(maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "currencies",
                schema: "ug",
                columns: table => new
                {
                    CurrencyId = table.Column<string>(maxLength: 200, nullable: false),
                    Code = table.Column<string>(maxLength: 10, nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    SymbolNative = table.Column<string>(nullable: true),
                    Rounding = table.Column<int>(nullable: false),
                    DecimalDigits = table.Column<int>(nullable: false),
                    NamePlural = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currencies", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "payment_methods",
                schema: "ug",
                columns: table => new
                {
                    PaymentMethodId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    ApiKey = table.Column<string>(nullable: true),
                    Secret = table.Column<string>(nullable: true),
                    Secret2 = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_methods", x => x.PaymentMethodId);
                });

            migrationBuilder.CreateTable(
                name: "region_timelines",
                schema: "ug",
                columns: table => new
                {
                    TimelineId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_region_timelines", x => x.TimelineId);
                });

            migrationBuilder.CreateTable(
                name: "regions",
                schema: "ug",
                columns: table => new
                {
                    RegionId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Flags_Active = table.Column<bool>(nullable: true),
                    Flags_CanRaiseTourRequests = table.Column<bool>(nullable: true),
                    Flags_CanMakePayments = table.Column<bool>(nullable: true),
                    Flags_CanMakeReservations = table.Column<bool>(nullable: true),
                    Flags_CanRegisterUsers = table.Column<bool>(nullable: true),
                    Stats_RegisteredUsers = table.Column<int>(nullable: true),
                    Stats_RegisteredGuides = table.Column<int>(nullable: true),
                    Stats_ToursOverallCount = table.Column<int>(nullable: true),
                    CurrencyId = table.Column<string>(nullable: true),
                    TimelineId = table.Column<string>(nullable: true),
                    CountryId = table.Column<string>(nullable: false),
                    PaymentMethodId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regions", x => x.RegionId);
                    table.ForeignKey(
                        name: "FK_regions_countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "ug",
                        principalTable: "countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_regions_currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "ug",
                        principalTable: "currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_regions_payment_methods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "ug",
                        principalTable: "payment_methods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_regions_region_timelines_TimelineId",
                        column: x => x.TimelineId,
                        principalSchema: "ug",
                        principalTable: "region_timelines",
                        principalColumn: "TimelineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Balance",
                columns: table => new
                {
                    BalanceId = table.Column<string>(nullable: false),
                    Coins = table.Column<decimal>(nullable: false),
                    Bonus = table.Column<decimal>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    RegionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Balance", x => x.BalanceId);
                    table.ForeignKey(
                        name: "FK_Balance_regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "ug",
                        principalTable: "regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "campains",
                schema: "ug",
                columns: table => new
                {
                    CampainId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    ActiveFrom = table.Column<DateTime>(nullable: false),
                    ActiveUntil = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 4000, nullable: false),
                    DescriptionSEO = table.Column<string>(maxLength: 4000, nullable: false),
                    ImageUrl = table.Column<string>(maxLength: 2000, nullable: true),
                    DiscountPercentage = table.Column<int>(nullable: false),
                    Membership = table.Column<int>(nullable: false),
                    RegionId = table.Column<string>(nullable: false),
                    TimelineId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campains", x => x.CampainId);
                    table.ForeignKey(
                        name: "FK_campains_regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "ug",
                        principalTable: "regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_campains_region_timelines_TimelineId",
                        column: x => x.TimelineId,
                        principalSchema: "ug",
                        principalTable: "region_timelines",
                        principalColumn: "TimelineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "authors",
                schema: "ug",
                columns: table => new
                {
                    AuthorId = table.Column<string>(nullable: false),
                    BalanceId = table.Column<string>(nullable: true),
                    SubscriptionId = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    ProfileInfo_FirstName = table.Column<string>(maxLength: 200, nullable: true),
                    ProfileInfo_ImageUrl = table.Column<string>(maxLength: 2000, nullable: true),
                    ProfileInfo_PhoneNumber = table.Column<string>(maxLength: 20, nullable: true),
                    ProfileInfo_CreatedAt = table.Column<DateTime>(nullable: true),
                    ProfileInfo_UpdatedAt = table.Column<DateTime>(nullable: true),
                    Activity_LastActive = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.AuthorId);
                    table.ForeignKey(
                        name: "FK_authors_Balance_BalanceId",
                        column: x => x.BalanceId,
                        principalTable: "Balance",
                        principalColumn: "BalanceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                schema: "ug",
                columns: table => new
                {
                    SubscriptionId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    Membership = table.Column<int>(nullable: false),
                    AuthorId = table.Column<string>(nullable: false),
                    ActivatedOn = table.Column<DateTime>(nullable: false),
                    EndsOn = table.Column<DateTime>(nullable: false),
                    CreditCard_CardHolderName = table.Column<string>(maxLength: 200, nullable: true),
                    CreditCard_CardNumber = table.Column<string>(maxLength: 16, nullable: true),
                    CreditCard_ExpiryYear = table.Column<short>(nullable: true),
                    CreditCard_ExpiryMonth = table.Column<byte>(nullable: true),
                    CanAutoRenew = table.Column<bool>(nullable: false),
                    TransactionRef = table.Column<string>(nullable: true),
                    DiscountPercentage = table.Column<int>(nullable: false),
                    RegionId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_subscriptions_authors_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "ug",
                        principalTable: "authors",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_subscriptions_regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "ug",
                        principalTable: "regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tours",
                schema: "ug",
                columns: table => new
                {
                    TourId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 4000, nullable: false),
                    Seats = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: false),
                    Schedule_Type = table.Column<int>(nullable: true),
                    Schedule_ActiveFrom = table.Column<DateTime>(nullable: true),
                    Schedule_ActiveUntil = table.Column<DateTime>(nullable: true),
                    Schedule_StartTime = table.Column<TimeSpan>(nullable: true),
                    Schedule_EndTime = table.Column<TimeSpan>(nullable: true),
                    Schedule_NextRun = table.Column<DateTime>(nullable: true),
                    Stats_Rating = table.Column<int>(nullable: true),
                    Stats_Likes = table.Column<int>(nullable: true),
                    Stats_ReactionsCount = table.Column<int>(nullable: true),
                    Stats_ReviewsCount = table.Column<int>(nullable: true),
                    Stats_ReservedSeats = table.Column<int>(nullable: true),
                    Stats_MapItsCount = table.Column<int>(nullable: true),
                    Stats_Views = table.Column<int>(nullable: true),
                    Stats_SharedCount = table.Column<int>(nullable: true),
                    AuthorId = table.Column<string>(nullable: false),
                    RegionId = table.Column<string>(nullable: false),
                    TimelineId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tours", x => x.TourId);
                    table.ForeignKey(
                        name: "FK_tours_authors_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "ug",
                        principalTable: "authors",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tours_regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "ug",
                        principalTable: "regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tours_region_timelines_TimelineId",
                        column: x => x.TimelineId,
                        principalSchema: "ug",
                        principalTable: "region_timelines",
                        principalColumn: "TimelineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    ReviewId = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    AuthorId = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    TourId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Review_authors_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "ug",
                        principalTable: "authors",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Review_tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "ug",
                        principalTable: "tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tour_booking",
                schema: "ug",
                columns: table => new
                {
                    BookingId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    AuthorId = table.Column<string>(nullable: false),
                    TourId = table.Column<string>(nullable: true),
                    When = table.Column<DateTime>(nullable: false),
                    EnablePushNotification = table.Column<bool>(nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    RegionId = table.Column<string>(nullable: false),
                    SubscriptionId = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    TourId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_booking", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_tour_booking_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_booking_regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "ug",
                        principalTable: "regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_booking_subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "ug",
                        principalTable: "subscriptions",
                        principalColumn: "SubscriptionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tour_booking_tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "ug",
                        principalTable: "tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tour_booking_tours_TourId1",
                        column: x => x.TourId1,
                        principalSchema: "ug",
                        principalTable: "tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tour_reactions",
                schema: "ug",
                columns: table => new
                {
                    ReactionId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    AuthorId = table.Column<string>(nullable: false),
                    TourId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_reactions", x => x.ReactionId);
                    table.ForeignKey(
                        name: "FK_tour_reactions_tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "ug",
                        principalTable: "tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tours_map_pins",
                schema: "ug",
                columns: table => new
                {
                    MapPinId = table.Column<string>(nullable: false, defaultValueSql: "NEWID()"),
                    ImageUrl = table.Column<string>(maxLength: 2000, nullable: false),
                    Description = table.Column<string>(maxLength: 4000, nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    TourId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tours_map_pins", x => x.MapPinId);
                    table.ForeignKey(
                        name: "FK_tours_map_pins_tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "ug",
                        principalTable: "tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Balance_RegionId",
                table: "Balance",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_AuthorId",
                table: "Review",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_TourId",
                table: "Review",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_authors_BalanceId",
                schema: "ug",
                table: "authors",
                column: "BalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_authors_SubscriptionId",
                schema: "ug",
                table: "authors",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_campains_RegionId",
                schema: "ug",
                table: "campains",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_campains_TimelineId",
                schema: "ug",
                table: "campains",
                column: "TimelineId");

            migrationBuilder.CreateIndex(
                name: "IX_regions_CountryId",
                schema: "ug",
                table: "regions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_regions_CurrencyId",
                schema: "ug",
                table: "regions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_regions_PaymentMethodId",
                schema: "ug",
                table: "regions",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_regions_TimelineId",
                schema: "ug",
                table: "regions",
                column: "TimelineId");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_AuthorId",
                schema: "ug",
                table: "subscriptions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_RegionId",
                schema: "ug",
                table: "subscriptions",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_tour_booking_AuthorId",
                schema: "ug",
                table: "tour_booking",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_tour_booking_RegionId",
                schema: "ug",
                table: "tour_booking",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_tour_booking_SubscriptionId",
                schema: "ug",
                table: "tour_booking",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_tour_booking_TourId",
                schema: "ug",
                table: "tour_booking",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_tour_booking_TourId1",
                schema: "ug",
                table: "tour_booking",
                column: "TourId1");

            migrationBuilder.CreateIndex(
                name: "IX_tour_reactions_TourId",
                schema: "ug",
                table: "tour_reactions",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_tours_AuthorId",
                schema: "ug",
                table: "tours",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_tours_RegionId",
                schema: "ug",
                table: "tours",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_tours_TimelineId",
                schema: "ug",
                table: "tours",
                column: "TimelineId");

            migrationBuilder.CreateIndex(
                name: "IX_tours_map_pins_TourId",
                schema: "ug",
                table: "tours_map_pins",
                column: "TourId");

            migrationBuilder.AddForeignKey(
                name: "FK_authors_subscriptions_SubscriptionId",
                schema: "ug",
                table: "authors",
                column: "SubscriptionId",
                principalSchema: "ug",
                principalTable: "subscriptions",
                principalColumn: "SubscriptionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Balance_regions_RegionId",
                table: "Balance");

            migrationBuilder.DropForeignKey(
                name: "FK_subscriptions_regions_RegionId",
                schema: "ug",
                table: "subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_subscriptions_authors_AuthorId",
                schema: "ug",
                table: "subscriptions");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "campains",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tour_booking",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tour_reactions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tours_map_pins",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tours",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "regions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "countries",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "currencies",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "payment_methods",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "region_timelines",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "authors",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Balance");

            migrationBuilder.DropTable(
                name: "subscriptions",
                schema: "ug");
        }
    }
}
