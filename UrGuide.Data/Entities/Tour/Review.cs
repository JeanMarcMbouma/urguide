using System;

namespace UrGuide.Data.Entities.Tour
{
    public class Review 
    {
        public string ReviewId { get; set; }
        public string Text { get; set; }
        public virtual Entities.Users.Author Author { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
    }
}
