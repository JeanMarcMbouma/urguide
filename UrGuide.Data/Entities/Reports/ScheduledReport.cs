using System;

namespace UrGuide.Data.Entities.Reports
{
    public class ScheduledReport
    {
        public string ScheduleId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public ReportType ReportType { get; set; }
        public ReportFormat Format { get; set; }
        public string ParametersJson { get; set; }
        public ScheduleFrequency Frequency { get; set; }
        public string EmailRecipients { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastRunAt { get; set; }
        public DateTime? NextRunAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum ScheduleFrequency
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2,
        Quarterly = 3
    }
}
