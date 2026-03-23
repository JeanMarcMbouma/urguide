using System.Collections.Generic;

namespace UrGuide.Model.Templates
{
    public class CreateTourTemplateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal BasePrice { get; set; }
        public string CurrencyCode { get; set; }
        public int DefaultDurationMinutes { get; set; }
        public int DefaultMaxParticipants { get; set; }
        public string DefaultMeetingPoint { get; set; }
        public List<ItineraryItem> Itinerary { get; set; } = new List<ItineraryItem>();
        public List<string> IncludedItems { get; set; } = new List<string>();
        public List<string> ExcludedItems { get; set; } = new List<string>();
    }

    public class ItineraryItem
    {
        public int Order { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public string Location { get; set; }
    }
}
