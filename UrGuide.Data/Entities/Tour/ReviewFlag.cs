using System;

namespace UrGuide.Data.Entities.Tour
{
    public class ReviewFlag
    {
        public string ReviewFlagId { get; set; }
        public string ReviewId { get; set; }
        public string FlaggedBy { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public ReviewFlagStatus Status { get; set; } = ReviewFlagStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
        public string ResolvedBy { get; set; }

        public virtual Review Review { get; set; }
        public virtual Entities.Users.User FlaggedByUser { get; set; }
    }
}
