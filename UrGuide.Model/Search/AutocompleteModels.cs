using System.Collections.Generic;

namespace UrGuide.Model.Search
{
    public class AutocompleteRequest
    {
        public string Query { get; set; }
        public int Size { get; set; } = 10;
        public string Type { get; set; } = "all"; // all, posts, tours
    }

    public class AutocompleteResponse
    {
        public List<AutocompleteSuggestion> Suggestions { get; set; } = new List<AutocompleteSuggestion>();
    }

    public class AutocompleteSuggestion
    {
        public string Text { get; set; }
        public string Type { get; set; } // post, tour
        public double Score { get; set; }
    }
}
