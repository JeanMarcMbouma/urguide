using System;
using System.Collections.Generic;
using UrGuide.Data.Entities.Regions;

namespace UrGuide.Data.Entities.Tour
{
    public class Tour
    {
        public string TourId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Seats { get; set; }
        public string Tags { get; set; }

        public virtual Schedule Schedule { get; set; }

        public virtual TourStats Stats { get; set; }

        public string AuthorId { get; set; }
        public virtual Users.Author Author { get; set; }
        public string RegionId { get; set; }
        public virtual Region Region { get; set; }

        public virtual ICollection<Review> Reviews { get; protected set; } = new HashSet<Review>();
        public virtual ICollection<MapPin> MapPins { get; protected set; } = new HashSet<MapPin>();
        public virtual ICollection<Booking> Bookings { get; protected set; } = new HashSet<Booking>();
        public virtual ICollection<Reaction> Reactions { get; protected set; } = new HashSet<Reaction>();
    }
}
