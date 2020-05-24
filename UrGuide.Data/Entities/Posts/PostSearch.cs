using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Data.Entities.Posts
{
    public class PostSearch
    {
        public string PostId { get; set; }
        public int Rating { get; set; }
        public DateTime EndDate { get; set; }
        public Point Location { get; set; }
    }
}
