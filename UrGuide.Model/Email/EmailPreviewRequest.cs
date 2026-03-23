using System.Collections.Generic;

namespace UrGuide.Model.Email
{
    public class EmailPreviewRequest
    {
        public string TemplateId { get; set; }
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();
    }
}
