using System;
using System.Collections.Generic;
using UrGuide.Model.Shared;

namespace UrGuide.Model.Posts
{
    public class PostCreationModel
    {
        public PostCreationModel()
        {
            Images = new HashSet<ImageFileModel>();
            Categories = new HashSet<string>();
        }

        public string Text { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UnitPrice { get; set; }
        public int Seats { get; set; }
        public string GeoLocation { get; set; }
        public ICollection<string> Categories { get; set; }
        public ICollection<ImageFileModel> Images { get; set; }
        public ImageFileModel Video { get; set; }
    }
}
