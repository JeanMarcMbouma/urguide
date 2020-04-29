using System.Collections.Generic;
using UrGuide.Model.Shared;

namespace UrGuide.Model.Posts
{
    public class PostModel
    {
        public PostModel()
        {
            Images = new HashSet<ImageFileCreateModel>();
            Categories = new HashSet<string>();
        }
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Rating { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string PublicationDate { get; set; }
        public string LastEditDate { get; set; }
        public string StartingBid { get; set; }
        public string LastBid { get; set; }
        public string Status { get; set; }
        public int Seats { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }

        public ICollection<string> Categories { get; protected set; }

        public ICollection<ImageFileCreateModel> Images { get; protected set; }
    }
}
