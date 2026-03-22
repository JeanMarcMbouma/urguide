using System;

namespace UrGuide.Data.Entities.Tour
{
    public class ReviewModerationAction
    {
        public string ActionId { get; set; }
        public string ReviewId { get; set; }
        public ModerationActionType ActionType { get; set; }
        public string PerformedBy { get; set; }
        public string Reason { get; set; }
        public string PreviousContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Review Review { get; set; }
        public virtual Entities.Users.User PerformedByUser { get; set; }
    }
}
