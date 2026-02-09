using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UrGuide.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ug");

            migrationBuilder.CreateTable(
                name: "Audit_Events",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    EventCode = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                schema: "ug",
                columns: table => new
                {
                    CountryId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DialCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false)
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
                    CurrencyId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SymbolNative = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rounding = table.Column<int>(type: "int", nullable: false),
                    DecimalDigits = table.Column<int>(type: "int", nullable: false),
                    NamePlural = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
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
                    PaymentMethodId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Secret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Secret2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_methods", x => x.PaymentMethodId);
                });

            migrationBuilder.CreateTable(
                name: "Post_Categories",
                schema: "ug",
                columns: table => new
                {
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImageLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Archived = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "region_timelines",
                schema: "ug",
                columns: table => new
                {
                    TimelineId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_region_timelines", x => x.TimelineId);
                });

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

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "ug",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastActivityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<Point>(type: "geography", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, defaultValue: "N/A"),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, defaultValue: "N/A"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, defaultValue: "N/A"),
                    UserName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StripeCustomerId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "regions",
                schema: "ug",
                columns: table => new
                {
                    RegionId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Flags_Active = table.Column<bool>(type: "bit", nullable: true),
                    Flags_CanRaiseTourRequests = table.Column<bool>(type: "bit", nullable: true),
                    Flags_CanMakePayments = table.Column<bool>(type: "bit", nullable: true),
                    Flags_CanMakeReservations = table.Column<bool>(type: "bit", nullable: true),
                    Flags_CanRegisterUsers = table.Column<bool>(type: "bit", nullable: true),
                    Stats_RegisteredUsers = table.Column<int>(type: "int", nullable: true),
                    Stats_RegisteredGuides = table.Column<int>(type: "int", nullable: true),
                    Stats_ToursOverallCount = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    TimelineId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CountryId = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    PaymentMethodId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                        principalColumn: "CurrencyId");
                    table.ForeignKey(
                        name: "FK_regions_payment_methods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "ug",
                        principalTable: "payment_methods",
                        principalColumn: "PaymentMethodId");
                    table.ForeignKey(
                        name: "FK_regions_region_timelines_TimelineId",
                        column: x => x.TimelineId,
                        principalSchema: "ug",
                        principalTable: "region_timelines",
                        principalColumn: "TimelineId");
                });

            migrationBuilder.CreateTable(
                name: "DataExportRequests",
                schema: "ug",
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
                name: "Image_Catalogs",
                schema: "ug",
                columns: table => new
                {
                    Image_CatalogId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Location = table.Column<Point>(type: "geography", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image_Catalogs", x => x.Image_CatalogId);
                    table.ForeignKey(
                        name: "FK_Image_Catalogs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "User_Attributes",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Attributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Attributes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Feedback",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FK_User_Feedback_Users = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Feedback_Users_FK_User_Feedback_Users",
                        column: x => x.FK_User_Feedback_Users,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_User_Feedback_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Images",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    ImageBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Images_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Notifications",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    FK_User_Notification_Users = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReferenceLink = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Read = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Notifications_Users_FK_User_Notification_Users",
                        column: x => x.FK_User_Notification_Users,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_User_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "author_balance",
                schema: "ug",
                columns: table => new
                {
                    BalanceId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Coins = table.Column<double>(type: "float", nullable: false),
                    Bonus = table.Column<double>(type: "float", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_author_balance", x => x.BalanceId);
                    table.ForeignKey(
                        name: "FK_author_balance_regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "ug",
                        principalTable: "regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campains",
                schema: "ug",
                columns: table => new
                {
                    CampainId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    ActiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActiveUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DescriptionSEO = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DiscountPercentage = table.Column<int>(type: "int", nullable: false),
                    Membership = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimelineId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campains", x => x.CampainId);
                    table.ForeignKey(
                        name: "FK_campains_region_timelines_TimelineId",
                        column: x => x.TimelineId,
                        principalSchema: "ug",
                        principalTable: "region_timelines",
                        principalColumn: "TimelineId");
                    table.ForeignKey(
                        name: "FK_campains_regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "ug",
                        principalTable: "regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Image_Catalog_Files",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    FileBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image_CatalogId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image_Catalog_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image_Catalog_Files_Image_Catalogs_Image_CatalogId",
                        column: x => x.Image_CatalogId,
                        principalSchema: "ug",
                        principalTable: "Image_Catalogs",
                        principalColumn: "Image_CatalogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Image_Catalogs_Attributes",
                schema: "ug",
                columns: table => new
                {
                    ImageCatalogId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image_Catalogs_Attributes", x => new { x.ImageCatalogId, x.Id });
                    table.ForeignKey(
                        name: "FK_Image_Catalogs_Attributes_Image_Catalogs_ImageCatalogId",
                        column: x => x.ImageCatalogId,
                        principalSchema: "ug",
                        principalTable: "Image_Catalogs",
                        principalColumn: "Image_CatalogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "ug",
                columns: table => new
                {
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Likes = table.Column<int>(type: "int", nullable: false),
                    Dislikes = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BidEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    Reviews = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AllocatedSeats = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ReservedSeats = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    GeoLocation = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Cost = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BidCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ItineraryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastBid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateOfPublication = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CatalogRef = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Location = table.Column<Point>(type: "geography", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_Image_Catalogs_CatalogRef",
                        column: x => x.CatalogRef,
                        principalSchema: "ug",
                        principalTable: "Image_Catalogs",
                        principalColumn: "Image_CatalogId");
                    table.ForeignKey(
                        name: "FK_Posts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "File_Attributes",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File_Attributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_File_Attributes_Image_Catalog_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "ug",
                        principalTable: "Image_Catalog_Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Bids",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    FK_Post_Bids_Users = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Bids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Bids_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Bids_Users_FK_Post_Bids_Users",
                        column: x => x.FK_Post_Bids_Users,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Bids_History",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FK_Post_Bids_History_Users = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Bids_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Bids_History_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Bids_History_Users_FK_Post_Bids_History_Users",
                        column: x => x.FK_Post_Bids_History_Users,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Feedback",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FK_Post_Feedback_Users = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Feedback_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Feedback_Users_FK_Post_Feedback_Users",
                        column: x => x.FK_Post_Feedback_Users,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Itineraries",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Ordinal = table.Column<byte>(type: "tinyint", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Itineraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Itineraries_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_UserReactions",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_UserReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_UserReactions_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_UserReactions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seat_Reservations",
                schema: "ug",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seat_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seat_Reservations_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "ug",
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Seat_Reservations_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ug",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "authors",
                schema: "ug",
                columns: table => new
                {
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BalanceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SubscriptionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ProfileInfo_FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProfileInfo_ImageUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProfileInfo_PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProfileInfo_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfileInfo_UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activity_LastActive = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.AuthorId);
                    table.ForeignKey(
                        name: "FK_authors_author_balance_BalanceId",
                        column: x => x.BalanceId,
                        principalSchema: "ug",
                        principalTable: "author_balance",
                        principalColumn: "BalanceId");
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
                name: "subscriptions",
                schema: "ug",
                columns: table => new
                {
                    SubscriptionId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Membership = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActivatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreditCard_CardHolderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreditCard_CardNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    CreditCard_ExpiryYear = table.Column<short>(type: "smallint", nullable: true),
                    CreditCard_ExpiryMonth = table.Column<byte>(type: "tinyint", nullable: true),
                    CanAutoRenew = table.Column<bool>(type: "bit", nullable: false),
                    TransactionRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscountPercentage = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    TourId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Schedule_Type = table.Column<int>(type: "int", nullable: true),
                    Schedule_ActiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Schedule_ActiveUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Schedule_StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Schedule_EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Schedule_NextRun = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Stats_Rating = table.Column<int>(type: "int", nullable: true),
                    Stats_Likes = table.Column<int>(type: "int", nullable: true),
                    Stats_ReactionsCount = table.Column<int>(type: "int", nullable: true),
                    Stats_ReviewsCount = table.Column<int>(type: "int", nullable: true),
                    Stats_ReservedSeats = table.Column<int>(type: "int", nullable: true),
                    Stats_MapItsCount = table.Column<int>(type: "int", nullable: true),
                    Stats_Views = table.Column<int>(type: "int", nullable: true),
                    Stats_SharedCount = table.Column<int>(type: "int", nullable: true),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimelineId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                        name: "FK_tours_region_timelines_TimelineId",
                        column: x => x.TimelineId,
                        principalSchema: "ug",
                        principalTable: "region_timelines",
                        principalColumn: "TimelineId");
                    table.ForeignKey(
                        name: "FK_tours_regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "ug",
                        principalTable: "regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                schema: "ug",
                columns: table => new
                {
                    ReviewId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TourId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_reviews_authors_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "ug",
                        principalTable: "authors",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reviews_tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "ug",
                        principalTable: "tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "tour_booking",
                schema: "ug",
                columns: table => new
                {
                    BookingId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TourId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    When = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnablePushNotification = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubscriptionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TourId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                        principalColumn: "SubscriptionId");
                    table.ForeignKey(
                        name: "FK_tour_booking_tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "ug",
                        principalTable: "tours",
                        principalColumn: "TourId");
                    table.ForeignKey(
                        name: "FK_tour_booking_tours_TourId1",
                        column: x => x.TourId1,
                        principalSchema: "ug",
                        principalTable: "tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "tour_reactions",
                schema: "ug",
                columns: table => new
                {
                    ReactionId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    AuthorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TourId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    MapPinId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    ImageUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    TourId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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

            migrationBuilder.InsertData(
                schema: "ug",
                table: "Post_Categories",
                columns: new[] { "CategoryId", "Archived", "Created", "ImageLink", "LastUpdated", "CategoryName" },
                values: new object[,]
                {
                    { "057e7c41-48a2-40af-83f7-86495daa66bb", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/child.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Child" },
                    { "3f35dba7-d527-4c70-80cb-68d25ee2b332", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/extreme.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Extreme" },
                    { "4dc654b1-c887-4000-8e53-309f2aad0e3d", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/historical.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Historical" },
                    { "62cf86ff-755d-46fd-bf8d-ca08ba353451", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/nature.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Nature" },
                    { "9d78cfc4-2299-445c-9c38-d6dd9d081f2b", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/amusement.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Amusement" },
                    { "d1442a22-adc5-4eab-a232-6ae1fe1ad4f5", false, new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "images/sport.png", new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Sport" }
                });

            migrationBuilder.InsertData(
                schema: "ug",
                table: "Users",
                columns: new[] { "UserId", "LastActivityDate", "Location", "StripeCustomerId", "UserName" },
                values: new object[] { "00000000-0000-0000-0000-000000000000", new DateTime(2020, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_author_balance_RegionId",
                schema: "ug",
                table: "author_balance",
                column: "RegionId");

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
                name: "IX_DataExportRequests_DownloadToken",
                schema: "ug",
                table: "DataExportRequests",
                column: "DownloadToken",
                unique: true,
                filter: "[DownloadToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DataExportRequests_ExpiresAt",
                schema: "ug",
                table: "DataExportRequests",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_DataExportRequests_Status",
                schema: "ug",
                table: "DataExportRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DataExportRequests_UserId",
                schema: "ug",
                table: "DataExportRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_File_Attributes_FileId",
                schema: "ug",
                table: "File_Attributes",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_Catalog_Files_Image_CatalogId",
                schema: "ug",
                table: "Image_Catalog_Files",
                column: "Image_CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_Catalogs_UserId",
                schema: "ug",
                table: "Image_Catalogs",
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
                name: "IX_Post_Bids_FK_Post_Bids_Users",
                schema: "ug",
                table: "Post_Bids",
                column: "FK_Post_Bids_Users");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Bids_PostId",
                schema: "ug",
                table: "Post_Bids",
                column: "PostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_Bids_History_FK_Post_Bids_History_Users",
                schema: "ug",
                table: "Post_Bids_History",
                column: "FK_Post_Bids_History_Users");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Bids_History_PostId",
                schema: "ug",
                table: "Post_Bids_History",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Feedback_FK_Post_Feedback_Users",
                schema: "ug",
                table: "Post_Feedback",
                column: "FK_Post_Feedback_Users");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Feedback_PostId",
                schema: "ug",
                table: "Post_Feedback",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Itineraries_PostId",
                schema: "ug",
                table: "Post_Itineraries",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserReactions_PostId",
                schema: "ug",
                table: "Post_UserReactions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserReactions_UserId",
                schema: "ug",
                table: "Post_UserReactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CatalogRef",
                schema: "ug",
                table: "Posts",
                column: "CatalogRef",
                unique: true,
                filter: "[CatalogRef] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                schema: "ug",
                table: "Posts",
                column: "UserId");

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
                name: "IX_reviews_AuthorId",
                schema: "ug",
                table: "reviews",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_TourId",
                schema: "ug",
                table: "reviews",
                column: "TourId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Seat_Reservations_PostId",
                schema: "ug",
                table: "Seat_Reservations",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_Reservations_UserId",
                schema: "ug",
                table: "Seat_Reservations",
                column: "UserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_User_Attributes_UserId",
                schema: "ug",
                table: "User_Attributes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Feedback_FK_User_Feedback_Users",
                schema: "ug",
                table: "User_Feedback",
                column: "FK_User_Feedback_Users");

            migrationBuilder.CreateIndex(
                name: "IX_User_Feedback_UserId",
                schema: "ug",
                table: "User_Feedback",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Images_UserId",
                schema: "ug",
                table: "User_Images",
                column: "UserId",
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_authors_subscriptions_SubscriptionId",
                schema: "ug",
                table: "authors",
                column: "SubscriptionId",
                principalSchema: "ug",
                principalTable: "subscriptions",
                principalColumn: "SubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_author_balance_regions_RegionId",
                schema: "ug",
                table: "author_balance");

            migrationBuilder.DropForeignKey(
                name: "FK_subscriptions_regions_RegionId",
                schema: "ug",
                table: "subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_authors_author_balance_BalanceId",
                schema: "ug",
                table: "authors");

            migrationBuilder.DropForeignKey(
                name: "FK_authors_subscriptions_SubscriptionId",
                schema: "ug",
                table: "authors");

            migrationBuilder.DropTable(
                name: "Audit_Events",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "campains",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "DataExportRequests",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "File_Attributes",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Image_Catalogs_Attributes",
                schema: "ug");

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
                name: "Post_Bids",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_Bids_History",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_Categories",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_Feedback",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_Itineraries",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Post_UserReactions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "refunds",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "reviews",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "SearchAnalytics",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Seat_Reservations",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tour_reactions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tour_requests",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tours_map_pins",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "User_Attributes",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "User_Feedback",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "User_Images",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "User_Notifications",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Image_Catalog_Files",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tour_booking",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Image_Catalogs",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "tours",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "Users",
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
                name: "author_balance",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "subscriptions",
                schema: "ug");

            migrationBuilder.DropTable(
                name: "authors",
                schema: "ug");
        }
    }
}
