using System;

namespace UrGuide.Model.Reports
{
    public class ReportListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
