using System;

namespace UrGuide.WebApp.Models
{
    public class Post
    {
        public long Id { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }
    }
}
