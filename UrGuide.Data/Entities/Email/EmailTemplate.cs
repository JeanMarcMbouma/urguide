using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Email
{
    public class EmailTemplate
    {
        public string TemplateId { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string PlainTextBody { get; set; }
        public string Category { get; set; }
        public string Language { get; set; } = "en";
        public int Version { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;
        public string VariablesJson { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<EmailTemplateVersion> Versions { get; set; } = new List<EmailTemplateVersion>();
    }
}
