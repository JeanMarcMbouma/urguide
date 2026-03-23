namespace UrGuide.Model.Reviews
{
    public class ReviewModerationResult
    {
        public string ReviewId { get; set; }
        /// <summary>
        /// 0 = Approved, 1 = Rejected, 2 = FlaggedForReview, 3 = Edited, 4 = Removed, 5 = Restored
        /// </summary>
        public int ActionType { get; set; }
        public string Reason { get; set; }
    }
}
