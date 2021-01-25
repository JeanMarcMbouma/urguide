using System.Collections.Generic;

namespace UrGuide.Data.Entities.Regions
{
    public class Country
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string DialCode { get; set; }

        public virtual ICollection<Region> Regions { get; private set; } = new HashSet<Region>();
    }
}
