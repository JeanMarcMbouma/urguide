using System;

namespace UrGuide.Model.Reports
{
    public class ReportDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RequestedBy { get; set; }
        public int Type { get; set; }
        public int Format { get; set; }
        public int Status { get; set; }
        public string FileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
