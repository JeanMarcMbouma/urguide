using System.Collections.Generic;
using UrGuide.Data.Entities.Attributes;

namespace UrGuide.Data.Entities.Shared
{
    public class Image
    {
        public Image()
        {
            Attributes = new List<GenericAttribute>();
        }
        public string Id { get; set; }
        public string ImageBase64 { get; set; }
        public string MimeType { get; set; }
        public virtual ImageCatalog ImageCatalog { get; set; }
        public virtual ICollection<GenericAttribute> Attributes { get; protected set; }
    }
}