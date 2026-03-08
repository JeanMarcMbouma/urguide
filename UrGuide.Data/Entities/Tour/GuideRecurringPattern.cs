using System;

namespace UrGuide.Data.Entities.Tour
{
    public class GuideRecurringPattern
    {
        public string Id { get; set; }
        public string GuideId { get; set; }
        public virtual Users.User Guide { get; set; }
        public string PatternType { get; set; } // "weekly" or "monthly"
        public int? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
