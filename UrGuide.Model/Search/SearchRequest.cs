using System;
using System.Collections.Generic;

namespace UrGuide.Model.Search
{
    public class SearchRequest
    {
        public string Query { get; set; }
        
        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        
        // Search type
        public bool FuzzySearch { get; set; } = true;
        public int Fuzziness { get; set; } = 2; // AUTO, 0, 1, 2
        
        // Filters
        public SearchFilters Filters { get; set; } = new SearchFilters();
        
        // Sorting
        public string SortBy { get; set; } = "relevance"; // relevance, date, rating, price
        public string SortOrder { get; set; } = "desc"; // asc, desc
        
        // Facets
        public bool IncludeFacets { get; set; } = true;
    }

    public class SearchFilters
    {
        // Location filters
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Distance { get; set; } // e.g., "50km", "100mi"
        
        // Price filter
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        
        // Rating filter
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
        
        // Date filters
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        
        // Tags filter
        public List<string> Tags { get; set; } = new List<string>();
        
        // Availability
        public bool? AvailableSeatsOnly { get; set; }
        public int? MinSeats { get; set; }
        
        // Other filters
        public bool? BidEnabled { get; set; }
        public string UserId { get; set; }
    }
}
