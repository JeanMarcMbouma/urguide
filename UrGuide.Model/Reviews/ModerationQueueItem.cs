using System;

namespace UrGuide.Model.Reviews
{
    public class ModerationQueueItem
    {
        public string ReviewId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 0 = Pending, 1 = Approved, 2 = Rejected, 3 = FlaggedForReview, 4 = Removed
        /// </summary>
        public int ModerationStatus { get; set; }
        public int FlagCount { get; set; }
        public decimal SpamScore { get; set; }
        public bool IsSpam { get; set; }
    }
}
