using System;
using System.Collections.Generic;
using UrGuide.Model.Shared;

namespace UrGuide.Model.Posts
{
    public class PostCreationModel
    {
        public PostCreationModel()
        {
            Images = new HashSet<ImageFileCreateModel>();
            Categories = new HashSet<string>();
            Itineraries = new HashSet<ItineraryModel>();
        }

        public string Text { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UnitPrice { get; set; }
        public int Seats { get; set; }
        public string GeoLocation { get; set; }
        public ICollection<string> Categories { get; set; }
        public ICollection<ImageFileCreateModel> Images { get; set; }
        public ImageFileCreateModel Video { get; set; }
        public ICollection<ItineraryModel> Itineraries { get; set; }
        public bool BidOptIn { get; set; }
    }
}
