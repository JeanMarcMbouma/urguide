using System;

namespace UrGuide.WebApp.Models
{
    public class Gallery
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }
    }
}
