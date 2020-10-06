using System.Collections.Generic;

namespace UrGuide.Model
{
    public class SearchParameters : PaginationParameters
    {
        public SearchParameters()
        {
            Extra = new HashSet<string>();
        }
        public string Term { get; set; }
        public IEnumerable<string> Extra { get; set; }
        public bool Nearby { get; set; }
    }
}
