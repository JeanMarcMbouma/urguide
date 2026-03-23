using System.Collections.Generic;

namespace UrGuide.Model.Email
{
    public class CreateEmailTemplateRequest
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string PlainTextBody { get; set; }
        public string Category { get; set; }
        public string Language { get; set; } = "en";
        public List<string> Variables { get; set; } = new List<string>();
    }
}
