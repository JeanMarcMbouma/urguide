using System;

namespace UrGuide.Data.Entities.Recommendations
{
    public class TourInteraction
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string TourId { get; set; }
        public InteractionType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum InteractionType
    {
        Viewed = 0,
        Bookmarked = 1,
        Booked = 2,
        Reviewed = 3,
        Shared = 4
    }
}
