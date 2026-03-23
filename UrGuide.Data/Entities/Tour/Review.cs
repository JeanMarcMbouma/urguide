using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Tour
{
    public class Review 
    {
        public string ReviewId { get; set; }
        public string Text { get; set; }
        public virtual Entities.Users.Author Author { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public ReviewModerationStatus ModerationStatus { get; set; } = ReviewModerationStatus.Pending;
        public bool IsSpam { get; set; } = false;
        public decimal SpamScore { get; set; } = 0;
        public virtual ICollection<ReviewFlag> Flags { get; set; } = new List<ReviewFlag>();
        public virtual ICollection<ReviewModerationAction> ModerationActions { get; set; } = new List<ReviewModerationAction>();
    }
}
