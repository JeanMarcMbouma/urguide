using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Shared
{
    public class ImageCatalog
    {
        public ImageCatalog()
        {
            Images = new List<Image>();
        }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public virtual Users.User User { get; set; }
        public virtual ICollection<Image> Images { get; protected set; }
    }
}