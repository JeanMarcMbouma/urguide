using System.Collections.Generic;

namespace UrGuide.Model.Disputes
{
    public class DisputeListResponse
    {
        public List<DisputeListItem> Disputes { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
