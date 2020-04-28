using System.Collections.Generic;
using UrGuide.Model.Shared;

namespace UrGuide.Model.Catalogs
{
    public class CreateImageCatalogModel
    {
        public CreateImageCatalogModel()
        {
            Files = new HashSet<ImageFileModel>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ImageFileModel> Files { get; set; }
    }
}
