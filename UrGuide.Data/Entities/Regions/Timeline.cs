using System.Collections.Generic;
using System.Linq;

namespace UrGuide.Data.Entities.Regions
{
    public class Timeline
    {
        public string TimelineId { get; set; }
        public virtual ICollection<Tour.Tour> Items { get; set; } = new HashSet<Tour.Tour>();
        public virtual ICollection<Tour.Campain> Campains { get; set; } = new HashSet<Tour.Campain>();

        public static implicit operator Tour.Timeline(Timeline item)
        {
            return new Tour.Timeline
            {
                Campains = new List<Tour.Campain>(item.Campains),
                Items = item.Items.ToList()
            };
        }
    }
}
