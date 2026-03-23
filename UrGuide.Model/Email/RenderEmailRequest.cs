using System.Collections.Generic;

namespace UrGuide.Model.Email
{
    public class RenderEmailRequest
    {
        public string TemplateId { get; set; }
        public string RecipientEmail { get; set; }
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();
    }
}
