using System.Collections.Generic;

namespace UrGuide.Model.Templates
{
    public class UpdateTourTemplateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal? BasePrice { get; set; }
        public string CurrencyCode { get; set; }
        public int? DefaultDurationMinutes { get; set; }
        public int? DefaultMaxParticipants { get; set; }
        public string DefaultMeetingPoint { get; set; }
        public List<ItineraryItem> Itinerary { get; set; }
        public List<string> IncludedItems { get; set; }
        public List<string> ExcludedItems { get; set; }
        public bool? IsActive { get; set; }
    }
}
