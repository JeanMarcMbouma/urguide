using System;
using UrGuide.Data.Entities.Regions;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Entities.Tour
{
    public class TourRequest
    {
        public string TourRequestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PreferredDate { get; set; }
        public int MaxParticipants { get; set; }
        public decimal MaxBudget { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public TourRequestStatus Status { get; set; }

        // Relationships
        public string RequesterId { get; set; }
        public virtual User Requester { get; set; }
        
        public string RegionId { get; set; }
        public virtual Region Region { get; set; }
    }

    public enum TourRequestStatus
    {
        Open = 0,
        InProgress = 1,
        Fulfilled = 2,
        Cancelled = 3,
        Expired = 4
    }
}