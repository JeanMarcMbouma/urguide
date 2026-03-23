using System.Collections.Generic;

namespace UrGuide.Model.Reports
{
    public class GuideEarningsReportData
    {
        public decimal TotalEarnings { get; set; }
        public int BookingCount { get; set; }
        public decimal AveragePerBooking { get; set; }
        public List<TopTourEarning> TopTours { get; set; } = new List<TopTourEarning>();
    }

    public class TopTourEarning
    {
        public string TourId { get; set; }
        public string TourName { get; set; }
        public decimal Earnings { get; set; }
        public int BookingCount { get; set; }
    }
}
