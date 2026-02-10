using System;
using System.Collections.Generic;

namespace UrGuide.Model.Analytics
{
    public class TourBookingStatistics
    {
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int PendingBookings { get; set; }
        public decimal AverageBookingValue { get; set; }
        public List<BookingDataPoint> TrendData { get; set; } = new List<BookingDataPoint>();
        public List<PopularTour> PopularTours { get; set; } = new List<PopularTour>();
    }

    public class BookingDataPoint
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }

    public class PopularTour
    {
        public string TourId { get; set; }
        public string TourTitle { get; set; }
        public int BookingCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
