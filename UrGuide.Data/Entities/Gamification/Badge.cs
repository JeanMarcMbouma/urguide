using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Gamification
{
    public enum BadgeTier
    {
        Silver = 0,
        Gold = 1,
        Platinum = 2
    }

    public class Badge
    {
        public string BadgeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public BadgeTier Tier { get; set; }
        public string Category { get; set; }
        public string Criteria { get; set; }
        public int ThresholdValue { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }
}
