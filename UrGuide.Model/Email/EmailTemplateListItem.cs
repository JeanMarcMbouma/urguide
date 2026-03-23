using System;

namespace UrGuide.Model.Email
{
    public class EmailTemplateListItem
    {
        public string TemplateId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
