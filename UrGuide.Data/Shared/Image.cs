using System.Collections.Generic;
using UrGuide.Core.Attributes;

namespace UrGuide.Data.Entities.Shared
{
    public class Image
    {
        public Image()
        {
            Attributes = new HashSet<GenericAttribute>();
        }
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string MimeType { get; set; }
        public virtual ICollection<GenericAttribute> Attributes { get; protected set; }

        public static implicit operator string(Image image) => image.ImageUrl;
    }
}