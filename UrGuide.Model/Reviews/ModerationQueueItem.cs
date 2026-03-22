using System;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Model.Reviews
{
    public class ModerationQueueItem
    {
        public string ReviewId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public ReviewModerationStatus ModerationStatus { get; set; }
        public int FlagCount { get; set; }
        public decimal SpamScore { get; set; }
        public bool IsSpam { get; set; }
    }
}
