using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Data.Entities.Posts
{
    public class Itinerary
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Ordinal { get; set; }
    }
}
