using System;

namespace UrGuide.Model.Email
{
    public class EmailTemplateVersionDto
    {
        public string VersionId { get; set; }
        public string TemplateId { get; set; }
        public int VersionNumber { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string PlainTextBody { get; set; }
        public string ChangeSummary { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
