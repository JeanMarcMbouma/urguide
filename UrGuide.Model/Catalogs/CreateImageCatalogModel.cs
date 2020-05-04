using System.Collections.Generic;
using UrGuide.Model.Shared;

namespace UrGuide.Model.Catalogs
{
    public class CreateImageCatalogModel
    {
        public CreateImageCatalogModel()
        {
            Files = new HashSet<ImageFileCreateModel>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ImageFileCreateModel> Files { get; set; }
    }
}
