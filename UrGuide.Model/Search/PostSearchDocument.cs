using System;
using System.Collections.Generic;
using Nest;

namespace UrGuide.Model.Search
{
    [ElasticsearchType(RelationName = "post")]
    public class PostSearchDocument
    {
        [Keyword]
        public string Id { get; set; }

        [Text(Analyzer = "standard")]
        [Completion]
        public string Text { get; set; }

        [Text(Analyzer = "standard")]
        public string Description { get; set; }

        [Keyword]
        public List<string> Tags { get; set; }

        [Text(Analyzer = "standard")]
        public string GeoLocation { get; set; }

        [GeoPoint]
        public GeoLocation Location { get; set; }

        [Keyword]
        public string Cost { get; set; }

        [Number(NumberType.Integer)]
        public int Rating { get; set; }

        [Number(NumberType.Integer)]
        public int Reviews { get; set; }

        [Number(NumberType.Integer)]
        public int AllocatedSeats { get; set; }

        [Number(NumberType.Integer)]
        public int ReservedSeats { get; set; }

        [Number(NumberType.Integer)]
        public int AvailableSeats { get; set; }

        [Date]
        public DateTime? StartDate { get; set; }

        [Date]
        public DateTime? EndDate { get; set; }

        [Boolean]
        public bool BidEnabled { get; set; }

        [Date]
        public DateTime DateOfPublication { get; set; }

        [Date]
        public DateTime LastUpdated { get; set; }

        [Keyword]
        public string UserId { get; set; }

        [Text(Analyzer = "standard")]
        public string UserName { get; set; }

        [Text(Analyzer = "standard")]
        public string UserFirstName { get; set; }

        [Text(Analyzer = "standard")]
        public string UserLastName { get; set; }

        [Number(NumberType.Integer)]
        public int Likes { get; set; }

        [Number(NumberType.Integer)]
        public int Dislikes { get; set; }
    }
}
