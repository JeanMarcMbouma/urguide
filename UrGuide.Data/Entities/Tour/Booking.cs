using System;
using UrGuide.Data.Entities.Regions;

namespace UrGuide.Data.Entities.Tour
{
    public class Booking
    {
        public string BookingId { get; set; }
        public string AuthorId { get; set; }
        public string TourId { get; set; }
        public virtual Users.User Author { get; set; }
        public virtual Tour Tour { get; set; }
        public DateTime When { get; set; }
        public bool EnablePushNotification { get; set; }
        public decimal Amount { get; set; }
        public string RegionId { get; set; }
        public virtual Region Region { get; set; }
        public virtual Subscription Subscription { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
