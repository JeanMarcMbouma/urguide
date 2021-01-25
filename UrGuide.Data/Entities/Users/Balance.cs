using System;
using UrGuide.Data.Entities.Regions;

namespace UrGuide.Data.Entities.Users
{
    public class Balance
    {
        public string BalanceId { get; set; }
        public decimal Coins { get; set; }
        public decimal Bonus { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Region Region { get; set; }
    }
}
