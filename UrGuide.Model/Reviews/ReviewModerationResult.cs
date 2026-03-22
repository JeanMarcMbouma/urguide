using UrGuide.Data.Entities.Tour;

namespace UrGuide.Model.Reviews
{
    public class ReviewModerationResult
    {
        public string ReviewId { get; set; }
        public ModerationActionType ActionType { get; set; }
        public string Reason { get; set; }
    }
}
