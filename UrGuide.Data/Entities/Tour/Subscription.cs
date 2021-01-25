using System;
using UrGuide.Data.Entities.Regions;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Entities.Tour
{
    public class Subscription
    {
        public string SubscriptionId { get; set; }
        public Membership Membership { get; set; }
        public string AuthorId { get; set; }
        public virtual Author Author { get; set; }
        public DateTime ActivatedOn { get; set; }
        public DateTime EndsOn { get; set; }
        public virtual CreditCardInfo CreditCard { get; set; }
        public bool CanAutoRenew { get; set; }
        public string TransactionRef { get; set; }
        public int DiscountPercentage { get; set; }
        public string RegionId { get; set; }
        public virtual Region Region { get; set; }
    }

}
