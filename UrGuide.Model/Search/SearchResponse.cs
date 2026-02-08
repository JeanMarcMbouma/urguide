using System.Collections.Generic;

namespace UrGuide.Model.Search
{
    public class SearchResponse<T>
    {
        public long TotalHits { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<SearchResultItem<T>> Results { get; set; } = new List<SearchResultItem<T>>();
        public SearchFacets Facets { get; set; }
        public long TimeTakenMs { get; set; }
    }

    public class SearchResultItem<T>
    {
        public T Document { get; set; }
        public double Score { get; set; }
        public Dictionary<string, List<string>> Highlights { get; set; }
    }

    public class SearchFacets
    {
        public Dictionary<string, long> TagsFacet { get; set; } = new Dictionary<string, long>();
        public Dictionary<string, long> LocationsFacet { get; set; } = new Dictionary<string, long>();
        public List<PriceRangeFacet> PriceRanges { get; set; } = new List<PriceRangeFacet>();
        public List<RatingFacet> RatingDistribution { get; set; } = new List<RatingFacet>();
        public Dictionary<string, long> DateRanges { get; set; } = new Dictionary<string, long>();
    }

    public class PriceRangeFacet
    {
        public string Range { get; set; }
        public long Count { get; set; }
        public decimal? From { get; set; }
        public decimal? To { get; set; }
    }

    public class RatingFacet
    {
        public int Rating { get; set; }
        public long Count { get; set; }
    }
}
