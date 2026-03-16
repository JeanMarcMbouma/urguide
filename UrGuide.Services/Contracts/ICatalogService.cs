using System.Collections.Generic;
using System.Threading;
using BbQ.Outcome;
using System.Threading.Tasks;
using UrGuide.Data.Entities.Posts;
using UrGuide.Model;
using UrGuide.Model.Catalogs;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;

namespace UrGuide.Services.Contracts
{
    public interface ICatalogService
    {
        Task<Outcome<IEnumerable<ImageCatalogModel>>> GetCatalogsAsync(string userId, CancellationToken cancellationToken);
        Task<Outcome<IEnumerable<ImageCatalogModel>>> GetCatalogsAsync(CancellationToken cancellationToken);
        Task<Outcome<ImageCatalogModel>> GetCatalogAsync(string catalogId, CancellationToken cancellationToken);
        Task<Outcome<ImageCatalogModel>> CreateCatalogAsync(CreateImageCatalogModel catalogModel, CancellationToken cancellationToken);
        Task<Outcome<bool>> AddCatalogToPostAsync(Post post, CreateImageCatalogModel catalogModel, CancellationToken cancellationToken);
        Task<Outcome<ImageFileModel>> AddImageToCatalogAsync(string catalogId, ImageFileCreateModel imageFile, CancellationToken cancellationToken);
        Task<Outcome<bool>> RemoveImageFromCatalogAsync(string catalogId, string[] imageIds, CancellationToken cancellationToken);
        Task<Outcome<bool>> SetCataglogAttributesAsync(string catalogId, SetAttribute[] attributes, CancellationToken cancellationToken);
        Task<Outcome<bool>> RemoveCatalogAsync(string catalogId, CancellationToken cancellationToken);
    }
}
