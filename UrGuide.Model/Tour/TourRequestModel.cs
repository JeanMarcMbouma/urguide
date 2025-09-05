using System;

namespace UrGuide.Model.Tour
{
    public class TourRequestModel
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
        
        public string RequesterId { get; set; }
        public string RequesterName { get; set; }
        
        public string RegionId { get; set; }
        public string RegionName { get; set; }
    }

    public class CreateTourRequestModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PreferredDate { get; set; }
        public int MaxParticipants { get; set; }
        public decimal MaxBudget { get; set; }
        public string Tags { get; set; }
        public string RegionId { get; set; }
    }

    public class UpdateBudgetModel
    {
        public decimal NewBudget { get; set; }
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