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
                .ForMember(x => x.Files, x => x.MapFrom(f => f.Images.Select(i => new ImageFileModel
                {
                    ImageBase64 = i.ImageBase64,
                    Name = i.Attributes.First(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Name))
                })));
        }
    }
}
