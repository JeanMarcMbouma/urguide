using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Disputes
{
    public class Dispute
    {
        public string DisputeId { get; set; }
        public string BookingId { get; set; }
        public string FiledBy { get; set; }
        public string AgainstUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DisputeCategory Category { get; set; }
        public DisputeStatus Status { get; set; } = DisputeStatus.Open;
        public DisputePriority Priority { get; set; } = DisputePriority.Medium;
        public string AssignedTo { get; set; }
        public string Resolution { get; set; }
        public decimal? RefundAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public virtual ICollection<DisputeEvidence> Evidence { get; set; } = new List<DisputeEvidence>();
        public virtual ICollection<DisputeMessage> Messages { get; set; } = new List<DisputeMessage>();
    }

    public enum DisputeStatus
    {
        Open = 0,
        UnderReview = 1,
        AwaitingResponse = 2,
        Resolved = 3,
        Closed = 4,
        Escalated = 5
    }

    public enum DisputeCategory
    {
        ServiceQuality = 0,
        Cancellation = 1,
        Pricing = 2,
        Safety = 3,
        Communication = 4,
        Other = 5
    }

    public enum DisputePriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Urgent = 3
    }
}
