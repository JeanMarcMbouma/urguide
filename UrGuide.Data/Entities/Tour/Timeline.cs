using System;
using System.Collections.Generic;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Entities.Tour
{
    public class Timeline
    {
        public string AuthorId { get; set; }
        public virtual Author Author { get; set; }
        public ICollection<Tour> Items { get; set; } = new HashSet<Tour>();
        public ICollection<Campain> Campains { get; set; } = new HashSet<Campain>();
        public DateTime UpdatedAt { get; set; }
    }
}
