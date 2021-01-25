using System;
using UrGuide.Data.Entities.Regions;

namespace UrGuide.Data.Entities.Tour
{
    public class Campain
    {
        public string CampainId { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveUntil { get; set; }
        public string Description { get; set; }
        public string DescriptionSEO { get; set; }
        public string ImageUrl { get; set; }
        public int DiscountPercentage { get; set; }
        public Membership Membership { get; set; }
        public string RegionId { get; set; }
        public virtual Region Region { get; set; }
    }

}
