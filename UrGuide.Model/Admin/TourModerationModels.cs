using System;

namespace UrGuide.Model.Admin
{
    /// <summary>
    /// Tour post information for moderation queue
    /// </summary>
    public class PendingTourModeration
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string GuideId { get; set; }
        public string GuideName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public decimal Cost { get; set; }
        public TourModerationStatus Status { get; set; }
        public string[] Tags { get; set; }
        public string[] Images { get; set; }
        public int ReportCount { get; set; }
    }

    /// <summary>
    /// Tour moderation status enum
    /// </summary>
    public enum TourModerationStatus
    {
        PendingReview = 0,
        UnderReview = 1,
        Approved = 2,
        Rejected = 3,
        Flagged = 4
    }

    /// <summary>
    /// Request to approve or reject tour post
    /// </summary>
    public class TourModerationDecisionModel
    {
        public string PostId { get; set; }
        public bool Approve { get; set; }
        public string Reason { get; set; }
        public string AdminNotes { get; set; }
        public bool NotifyGuide { get; set; }
    }

    /// <summary>
    /// Tour post detail for moderation review
    /// </summary>
    public class TourModerationDetail
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string GuideId { get; set; }
        public string GuideName { get; set; }
        public string GuideEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public decimal Cost { get; set; }
        public TourModerationStatus Status { get; set; }
        public string[] Tags { get; set; }
        public string[] Images { get; set; }
        public ContentViolation[] Violations { get; set; }
        public int BidCount { get; set; }
        public int ReservationCount { get; set; }
        public int ReportCount { get; set; }
        public string[] Itinerary { get; set; }
    }

    /// <summary>
    /// Content violation information
    /// </summary>
    public class ContentViolation
    {
        public string ViolationType { get; set; }
        public string Description { get; set; }
        public string ReportedBy { get; set; }
        public DateTime ReportedAt { get; set; }
        public bool Reviewed { get; set; }
    }
}
