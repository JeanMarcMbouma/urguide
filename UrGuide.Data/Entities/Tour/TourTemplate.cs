using System;

namespace UrGuide.Data.Entities.Tour
{
    public class TourTemplate
    {
        public string TemplateId { get; set; }
        public string GuideId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal BasePrice { get; set; }
        public string CurrencyCode { get; set; }
        public int DefaultDurationMinutes { get; set; }
        public int DefaultMaxParticipants { get; set; }
        public string DefaultMeetingPoint { get; set; }
        public string ItineraryJson { get; set; }
        public string IncludedItemsJson { get; set; }
        public string ExcludedItemsJson { get; set; }
        public bool IsActive { get; set; } = true;
        public int UsageCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
    }
}
