using System;

namespace UrGuide.Data.Entities.Gamification
{
    public class UserAchievement
    {
        public string UserAchievementId { get; set; }
        public string UserId { get; set; }
        public string AchievementId { get; set; }
        public virtual Achievement Achievement { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
