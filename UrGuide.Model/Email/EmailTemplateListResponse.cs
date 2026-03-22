using System.Collections.Generic;

namespace UrGuide.Model.Email
{
    public class EmailTemplateListResponse
    {
        public List<EmailTemplateListItem> Templates { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
