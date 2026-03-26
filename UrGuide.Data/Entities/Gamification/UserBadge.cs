using System;

namespace UrGuide.Data.Entities.Gamification
{
    public class UserBadge
    {
        public string UserBadgeId { get; set; }
        public string UserId { get; set; }
        public string BadgeId { get; set; }
        public virtual Badge Badge { get; set; }
        public DateTime EarnedAt { get; set; }
    }
}
