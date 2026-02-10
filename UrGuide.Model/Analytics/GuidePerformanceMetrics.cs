using System;
using System.Collections.Generic;

namespace UrGuide.Model.Analytics
{
    public class GuidePerformanceMetrics
    {
        public int TotalGuides { get; set; }
        public int ActiveGuides { get; set; }
        public decimal AverageRating { get; set; }
        public List<TopGuide> TopPerformers { get; set; } = new List<TopGuide>();
    }

    public class TopGuide
    {
        public string GuideId { get; set; }
        public string GuideName { get; set; }
        public int TotalTours { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
