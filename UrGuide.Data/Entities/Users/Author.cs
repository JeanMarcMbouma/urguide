using System;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Entities.Users
{
    public class Author
    {
        public string AuthorId { get; set; }
        public string BalanceId { get; set; }
        public virtual Balance Balance { get; set; }
        public string SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; }
        public int Rating { get; set; }
        public virtual AuthorProfile ProfileInfo { get; set; }
        public virtual AuthorActivity Activity { get; set; }
    }

    public class AuthorProfile
    {
        public string FirstName { get; set; }
        public string ImageUrl { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AuthorActivity
    {
        public DateTime LastActive { get; set; }
    }
}
