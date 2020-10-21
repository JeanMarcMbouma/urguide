using MvvmHelpers;
using System.Linq;
using UrGuide.Model.Shared;

namespace UrGuide.Mobile.Models
{
    public class GalleryItem : ObservableObject
    {
        public string CatalogId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Image => Files.FirstOrDefault()?.ImageBase64;
        public ObservableRangeCollection<ImageFileModel> Files { get; set; } = new ObservableRangeCollection<ImageFileModel>();
    }
}
