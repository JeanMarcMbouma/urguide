using AutoMapper;
using System.Linq;
using UrGuide.Model.Shared;

namespace UrGuide.Services.Catalogs
{
    class CatalogMap : Profile
    {
        public CatalogMap()
        {
            CreateMap<Data.Entities.Shared.ImageCatalog, Model.Catalogs.ImageCatalogModel>()
                .ForMember(x => x.Description, x => x.MapFrom(f => f.Attributes.First(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Description))))
                .ForMember(x => x.Name, x => x.MapFrom(f => f.Attributes.First(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Name))))
                .ForMember(x => x.CatalogId, x => x.MapFrom(f => f.Id))
                .ForMember(x => x.AuthorId, x => x.MapFrom(p => p.User != null ? p.User.Id : Constants.EmptyGuid))
                .ForMember(x => x.Author, x => x.MapFrom(p => p.User != null ? p.User.FullName : Constants.Unknown))
                .ForMember(x => x.AuthorAvatar, x => x.MapFrom(p => p.User != null && p.User.ProfileImage != null ? p.User.ProfileImage.ImageUrl : Constants.UnknownImage))
                .ForMember(x => x.Files, x => x.MapFrom(f => f.Images.Select(i => new ImageFileModel
                {
                    Id = i.Id,
                    ImageBase64 = i.ImageUrl,
                    Name = i.Attributes.FirstOrDefault(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Name))
                })));

            CreateMap<Data.Entities.Shared.Image, Model.Shared.ImageFileModel>()
                .ForMember(x => x.Id, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.ImageBase64, y => y.MapFrom(x => x.ImageUrl))
                .ForMember(x => x.Name, y => y.MapFrom(x => x.Attributes.FirstOrDefault(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Name))));
        }
    }
}
