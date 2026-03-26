using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Financial
{
    public class CoinWallet
    {
        public string CoinWalletId { get; set; }
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalEarned { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<CoinTransaction> Transactions { get; set; } = new List<CoinTransaction>();
    }
}
