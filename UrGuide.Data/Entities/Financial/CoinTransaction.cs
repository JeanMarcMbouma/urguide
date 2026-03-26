using System;

namespace UrGuide.Data.Entities.Financial
{
    public enum CoinTransactionType
    {
        Purchase = 0,
        Reward = 1,
        Referral = 2,
        TourPayment = 3,
        Refund = 4,
        Withdrawal = 5,
        Bonus = 6
    }

    public class CoinTransaction
    {
        public string CoinTransactionId { get; set; }
        public string CoinWalletId { get; set; }
        public virtual CoinWallet CoinWallet { get; set; }
        public decimal Amount { get; set; }
        public CoinTransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public string ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
