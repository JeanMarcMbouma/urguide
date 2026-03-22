namespace UrGuide.Model.Email
{
    public class UpdateEmailTemplateRequest
    {
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string PlainTextBody { get; set; }
        public string ChangeSummary { get; set; }
    }
}
