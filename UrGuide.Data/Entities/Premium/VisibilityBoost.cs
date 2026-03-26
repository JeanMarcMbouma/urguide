using System;

namespace UrGuide.Data.Entities.Premium
{
    public enum BoostType
    {
        SearchRanking = 0,
        FeaturedListing = 1,
        TopResult = 2,
        HighlightedProfile = 3
    }

    public enum BoostStatus
    {
        Active = 0,
        Expired = 1,
        Cancelled = 2
    }

    public class VisibilityBoost
    {
        public string VisibilityBoostId { get; set; }
        public string GuideId { get; set; }
        public string TourId { get; set; }
        public BoostType BoostType { get; set; }
        public BoostStatus Status { get; set; }
        public int BoostMultiplier { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Cost { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
