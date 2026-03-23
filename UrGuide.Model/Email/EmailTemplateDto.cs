using System;
using System.Collections.Generic;

namespace UrGuide.Model.Email
{
    public class EmailTemplateDto
    {
        public string TemplateId { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string PlainTextBody { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public List<string> Variables { get; set; } = new List<string>();
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
