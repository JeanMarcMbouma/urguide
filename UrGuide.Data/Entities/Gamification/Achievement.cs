using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Gamification
{
    public class Achievement
    {
        public string AchievementId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string Category { get; set; }
        public int ThresholdValue { get; set; }
        public int PointsReward { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    }
}
