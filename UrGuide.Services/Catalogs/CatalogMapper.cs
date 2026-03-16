using System.Linq;
using UrGuide.Model.Shared;

namespace UrGuide.Services.Catalogs
{
    static class CatalogMapper
    {
        public static Model.Catalogs.ImageCatalogModel ToImageCatalogModel(Data.Entities.Shared.ImageCatalog source)
        {
            return new Model.Catalogs.ImageCatalogModel
            {
                CatalogId = source.Id,
                Name = source.Attributes.First(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Name)),
                Description = source.Attributes.First(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Description)),
                AuthorId = source.User != null ? source.User.Id : Constants.EmptyGuid,
                Author = source.User != null ? source.User.FullName?.ToString() : Constants.Unknown,
                AuthorAvatar = source.User?.ProfileImage != null ? source.User.ProfileImage.ImageUrl : Constants.UnknownImage,
                Files = source.Images.Select(ToImageFileModel).ToList()
            };
        }

        public static ImageFileModel ToImageFileModel(Data.Entities.Shared.Image source)
        {
            return new ImageFileModel
            {
                Id = source.Id,
                ImageBase64 = source.ImageUrl,
                Name = source.Attributes.FirstOrDefault(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Name))
            };
        }
    }
}
