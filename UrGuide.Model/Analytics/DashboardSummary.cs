namespace UrGuide.Model.Analytics
{
    public class DashboardSummary
    {
        public UserRegistrationTrends UserTrends { get; set; }
        public TourBookingStatistics BookingStats { get; set; }
        public RevenueMetrics Revenue { get; set; }
        public GuidePerformanceMetrics GuideMetrics { get; set; }
        public PopularDestinations Destinations { get; set; }
        public ConversionFunnel Funnel { get; set; }
    }
}
