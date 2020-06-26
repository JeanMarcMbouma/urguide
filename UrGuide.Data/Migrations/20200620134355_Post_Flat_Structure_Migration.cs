using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Post_Flat_Structure_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE p SET p.AllocatedSeats = CAST(a1.Value AS INT),
	p.Reviews = CAST(a2.Value as INT),
	p.Likes = CAST(a3.Value as INT),
	p.Dislikes = CAST(a4.Value as INT),
	p.Cost = a5.Value,
	p.StartDate = CAST(a8.Value + ' ' + a9.Value AS datetime2),
	p.EndDate = CAST(a6.Value + ' ' + a7.Value AS datetime2),
	p.BidCount = (SELECT COUNT(1) FROM ug.Post_Bids_History WHERE PostId = p.PostId),
	p.BidEnabled = CASE WHEN a10.Value = 'Yes' THEN 1 ELSE 0 END,
	p.GeoLocation = a11.Value,
	p.ItineraryCount = (SELECT COUNT(1) FROM ug.Post_Itineraries WHERE PostId = p.PostId),
	p.ReservedSeats = CASE WHEN a13.Value IS NULL THEN 0 ELSE CAST(a13.Value AS INT) END,
	p.LastBid = a12.Value,
	p.Rating = CASE WHEN a14.Value = '0' THEN 5 ELSE CAST(a14.Value AS INT) END,
	p.Tags = a15.Value
FROM ug.Posts as p
JOIN ug.Post_Attributes a1 ON a1.PostId = p.PostId AND a1.Name = 'AllocatedSeats' 
JOIN ug.Post_Attributes a2 ON a2.PostId = p.PostId AND a2.Name = 'Reviews' 
JOIN ug.Post_Attributes a3 ON a3.PostId = p.PostId AND a3.Name LIKE 'Likes' 
JOIN ug.Post_Attributes a4 ON a4.PostId = p.PostId AND a4.Name LIKE 'Dislikes' 
JOIN ug.Post_Attributes a5 ON a5.PostId = p.PostId AND a5.Name LIKE 'Amount' 
JOIN ug.Post_Attributes a6 ON a6.PostId = p.PostId AND a6.Name LIKE 'DateEnd'
JOIN ug.Post_Attributes a7 ON a7.PostId = p.PostId AND a7.Name LIKE 'TimeEnd'
JOIN ug.Post_Attributes a8 ON a8.PostId = p.PostId AND a8.Name LIKE 'DateStart'
JOIN ug.Post_Attributes a9 ON a9.PostId = p.PostId AND a9.Name LIKE 'TimeStart'
LEFT JOIN ug.Post_Attributes a10 ON a10.PostId = p.PostId AND a10.Name LIKE 'BidOptIn' 
JOIN ug.Post_Attributes a11 ON a11.PostId = p.PostId AND a11.Name LIKE 'GeoLocation'
LEFT JOIN ug.Post_Attributes a12 ON a12.PostId = p.PostId AND a12.Name LIKE 'LastBid' 
LEFT JOIN ug.Post_Attributes a13 ON a13.PostId = p.PostId AND a13.Name LIKE 'ReservedSeats' 
JOIN ug.Post_Attributes a14 ON a14.PostId = p.PostId AND a14.Name LIKE 'Rating'  
JOIN ug.Post_Attributes a15 ON a15.PostId = p.PostId AND a15.Name LIKE 'Categories' 
");
			migrationBuilder.Sql("TRUNCATE TABLE ug.Post_Attributes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
