using System.Collections.Generic;
using UrGuide.Model.Shared;

namespace UrGuide.Model.Catalogs
{
    public class ImageCatalogModel
    {
        public string CatalogId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ImageFileCreateModel> Files { get; set; }
    }
}
