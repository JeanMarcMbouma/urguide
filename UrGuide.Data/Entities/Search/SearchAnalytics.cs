using System;

namespace UrGuide.Data.Entities.Search
{
    public class SearchAnalytics
    {
        public string Id { get; set; }
        public string Query { get; set; }
        public string UserId { get; set; }
        public DateTime SearchedAt { get; set; }
        public long ResultsCount { get; set; }
        public long TimeTakenMs { get; set; }
        public string Filters { get; set; } // JSON serialized filters
        public string SearchType { get; set; } // posts, tours, all
        public bool HasResults { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}
