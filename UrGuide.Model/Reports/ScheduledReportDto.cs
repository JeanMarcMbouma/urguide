using System;

namespace UrGuide.Model.Reports
{
    public class ScheduledReportDto
    {
        public string ScheduleId { get; set; }
        public string Name { get; set; }
        public int ReportType { get; set; }
        public int Format { get; set; }
        public int Frequency { get; set; }
        public string EmailRecipients { get; set; }
        public string Parameters { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastRunAt { get; set; }
        public DateTime? NextRunAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
