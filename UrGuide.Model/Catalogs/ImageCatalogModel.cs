using System.Collections.Generic;

namespace UrGuide.Model.Catalogs
{
    public class ImageCatalogModel
    {
        public string CatalogId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ImageFileModel> Files { get; set; }
    }
}
