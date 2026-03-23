using System;

namespace UrGuide.Data.Entities.Recommendations
{
    public class RecommendationLog
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string TourId { get; set; }
        public decimal Score { get; set; }
        public string Algorithm { get; set; } // "collaborative", "content-based", "popularity", "location"
        public bool WasClicked { get; set; } = false;
        public bool WasBooked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
