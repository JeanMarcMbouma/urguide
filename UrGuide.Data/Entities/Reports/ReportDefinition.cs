using System;

namespace UrGuide.Data.Entities.Reports
{
    public class ReportDefinition
    {
        public string ReportId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ReportType Type { get; set; }
        public string RequestedBy { get; set; }
        public ReportFormat Format { get; set; }
        public string ParametersJson { get; set; }
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public string FileUrl { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }

    public enum ReportType
    {
        GuideEarnings = 0,
        BookingSummary = 1,
        TaxDocument = 2,
        PerformanceMetrics = 3,
        CustomerSatisfaction = 4,
        PlatformRevenue = 5,
        OperationalOverview = 6
    }

    public enum ReportFormat
    {
        PDF = 0,
        Excel = 1,
        CSV = 2
    }

    public enum ReportStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3
    }
}
