using System;
using System.Collections.Generic;
using Nest;

namespace UrGuide.Model.Search
{
    [ElasticsearchType(RelationName = "tour")]
    public class TourSearchDocument
    {
        [Keyword]
        public string TourId { get; set; }

        [Text(Analyzer = "standard")]
        [Completion]
        public string Title { get; set; }

        [Text(Analyzer = "standard")]
        public string Description { get; set; }

        [Keyword]
        public List<string> Tags { get; set; }

        [Number(NumberType.Integer)]
        public int Seats { get; set; }

        [Date]
        public DateTime CreatedAt { get; set; }

        [Date]
        public DateTime UpdatedAt { get; set; }

        [Keyword]
        public string AuthorId { get; set; }

        [Text(Analyzer = "standard")]
        public string AuthorName { get; set; }

        [Keyword]
        public string RegionId { get; set; }

        [Text(Analyzer = "standard")]
        public string RegionName { get; set; }

        [Number(NumberType.Integer)]
        public int TotalReviews { get; set; }

        [Number(NumberType.Double)]
        public double AverageRating { get; set; }

        [Number(NumberType.Integer)]
        public int TotalBookings { get; set; }

        [Number(NumberType.Integer)]
        public int TotalReactions { get; set; }
    }
}
