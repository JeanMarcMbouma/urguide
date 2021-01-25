using Microsoft.EntityFrameworkCore;

namespace UrGuide.Data.Entities.Tour
{
    [Owned]
    public class TourStats
    {
        public int Rating { get; set; }
        public int Likes { get; set; }
        public int ReactionsCount { get; set; }
        public int ReviewsCount { get; set; }
        public int ReservedSeats { get; set; }
        public int MapItsCount { get; set; }
        public int Views { get; set; }
        public int SharedCount { get; set; }
    }
}
