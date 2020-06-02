using Microsoft.EntityFrameworkCore.Migrations;
using UrGuide.Data.Entities.Posts;

namespace UrGuide.Data.Migrations
{
    public partial class Adding_Columns_ToPostSearch_View : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"ALTER VIEW {Constants.Schema}.Post_Search AS
                    SELECT P.PostId, P.Location, P.UserId, PA3.Value AS Rating, ISNULL(TRY_CONVERT(DATETIME, PA1.Value + ' ' + PA2.Value), GETUTCDATE()) AS EndDate, PA4.Value AS GeoLocation 
                    FROM {Constants.Schema}.Posts P
                    JOIN {Constants.Schema}.Post_Attributes PA1 ON PA1.PostId = P.PostId AND PA1.Name LIKE '{nameof(AttributeTypes.DateEnd)}'
                    JOIN {Constants.Schema}.Post_Attributes PA2 ON PA2.PostId = P.PostId AND PA2.Name LIKE '{nameof(AttributeTypes.TimeEnd)}'
                    JOIN {Constants.Schema}.Post_Attributes PA3 ON PA3.PostId = P.PostId AND PA3.Name LIKE '{nameof(AttributeTypes.Rating)}'
                    JOIN {Constants.Schema}.Post_Attributes PA4 ON PA4.PostId = P.PostId AND PA4.Name LIKE '{nameof(AttributeTypes.GeoLocation)}'
                    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"ALTER VIEW {Constants.Schema}.Post_Search AS
                    SELECT P.PostId, P.Location, PA3.Value AS Rating, ISNULL(TRY_CONVERT(DATETIME, PA1.Value + ' ' + PA2.Value), GETUTCDATE()) AS EndDate 
                    FROM {Constants.Schema}.Posts P
                    JOIN {Constants.Schema}.Post_Attributes PA1 ON PA1.PostId = P.PostId AND PA1.Name LIKE '{nameof(AttributeTypes.DateEnd)}'
                    JOIN {Constants.Schema}.Post_Attributes PA2 ON PA2.PostId = P.PostId AND PA2.Name LIKE '{nameof(AttributeTypes.TimeEnd)}'
                    JOIN {Constants.Schema}.Post_Attributes PA3 ON PA3.PostId = P.PostId AND PA3.Name LIKE '{nameof(AttributeTypes.Rating)}'
                    ");
        }
    }
}
