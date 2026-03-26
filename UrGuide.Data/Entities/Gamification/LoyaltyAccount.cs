using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Gamification
{
    public enum LoyaltyTier
    {
        Bronze = 0,
        Silver = 1,
        Gold = 2,
        Platinum = 3
    }

    public class LoyaltyAccount
    {
        public string LoyaltyAccountId { get; set; }
        public string UserId { get; set; }
        public int Points { get; set; }
        public LoyaltyTier Tier { get; set; }
        public int DiscountPercentage { get; set; }
        public int TotalToursCompleted { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<LoyaltyTransaction> Transactions { get; set; } = new List<LoyaltyTransaction>();
    }
}
