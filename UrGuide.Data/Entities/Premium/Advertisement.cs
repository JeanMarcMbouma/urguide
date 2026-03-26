using System;

namespace UrGuide.Data.Entities.Premium
{
    public enum AdStatus
    {
        Draft = 0,
        Active = 1,
        Paused = 2,
        Expired = 3,
        Rejected = 4
    }

    public enum AdTargetAudience
    {
        AllUsers = 0,
        Tourists = 1,
        Guides = 2,
        PremiumUsers = 3,
        RegionSpecific = 4
    }

    public class Advertisement
    {
        public string AdvertisementId { get; set; }
        public string AdvertiserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string TargetUrl { get; set; }
        public AdTargetAudience TargetAudience { get; set; }
        public string TargetRegionId { get; set; }
        public AdStatus Status { get; set; }
        public decimal Budget { get; set; }
        public decimal SpentAmount { get; set; }
        public int Impressions { get; set; }
        public int Clicks { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
