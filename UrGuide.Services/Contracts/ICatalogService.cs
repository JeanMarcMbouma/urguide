using System.Collections.Generic;
using System.Threading;
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
        Task<Result<IEnumerable<ImageCatalogModel>>> GetCatalogsAsync(string userId, CancellationToken cancellationToken);
        Task<Result<IEnumerable<ImageCatalogModel>>> GetCatalogsAsync(CancellationToken cancellationToken);
        Task<Result<ImageCatalogModel>> GetCatalogAsync(string catalogId, CancellationToken cancellationToken);
        Task<Result<ImageCatalogModel>> CreateCatalogAsync(CreateImageCatalogModel catalogModel, CancellationToken cancellationToken);
        Task<Result<bool>> AddCatalogToPostAsync(Post post, CreateImageCatalogModel catalogModel, CancellationToken cancellationToken);
        Task<Result<ImageFileModel>> AddImageToCatalogAsync(string catalogId, ImageFileCreateModel imageFile, CancellationToken cancellationToken);
        Task<Result<bool>> RemoveImageFromCatalogAsync(string catalogId, string[] imageIds, CancellationToken cancellationToken);
        Task<Result<bool>> SetCataglogAttributesAsync(string catalogId, SetAttribute[] attributes, CancellationToken cancellationToken);
        Task<Result<bool>> RemoveCatalogAsync(string catalogId, CancellationToken cancellationToken);
    }
}
