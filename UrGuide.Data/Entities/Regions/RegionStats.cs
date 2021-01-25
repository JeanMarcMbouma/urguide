using Microsoft.EntityFrameworkCore;

namespace UrGuide.Data.Entities.Regions
{
    [Owned]
    public class RegionStats
    {
        public int RegisteredUsers { get; set; }
        public int RegisteredGuides { get; set; }
        public int ToursOverallCount { get; set; }
    }
}
