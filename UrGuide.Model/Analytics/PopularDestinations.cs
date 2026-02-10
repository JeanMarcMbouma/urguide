using System.Collections.Generic;

namespace UrGuide.Model.Analytics
{
    public class PopularDestinations
    {
        public List<DestinationMetric> Destinations { get; set; } = new List<DestinationMetric>();
    }

    public class DestinationMetric
    {
        public string RegionId { get; set; }
        public string RegionName { get; set; }
        public string CountryName { get; set; }
        public int TourCount { get; set; }
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageRating { get; set; }
    }
}
