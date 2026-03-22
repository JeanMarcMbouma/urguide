using System.Collections.Generic;

namespace UrGuide.Model.Reports
{
    public class ReportDataDto
    {
        public string ReportId { get; set; }
        public string ReportName { get; set; }
        public List<string> Headers { get; set; } = new List<string>();
        public List<List<string>> Rows { get; set; } = new List<List<string>>();
    }
}
