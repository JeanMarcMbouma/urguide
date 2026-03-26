using System;

namespace UrGuide.Data.Entities.Gamification
{
    public enum LoyaltyTransactionType
    {
        Earned = 0,
        Redeemed = 1,
        Expired = 2,
        Bonus = 3,
        TierUpgrade = 4
    }

    public class LoyaltyTransaction
    {
        public string LoyaltyTransactionId { get; set; }
        public string LoyaltyAccountId { get; set; }
        public virtual LoyaltyAccount LoyaltyAccount { get; set; }
        public int Points { get; set; }
        public LoyaltyTransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public string ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
