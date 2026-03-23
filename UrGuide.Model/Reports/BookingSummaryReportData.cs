namespace UrGuide.Model.Reports
{
    public class BookingSummaryReportData
    {
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal Revenue { get; set; }
    }
}
