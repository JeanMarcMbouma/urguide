using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Mobile.Models
{
    public class PostItem : Model.Posts.PostModel
    {
        public List<Model.Posts.ItineraryModel> Itineraries { get; set; }
    }
}
