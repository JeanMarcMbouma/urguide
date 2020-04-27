using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model;
using UrGuide.Model.Catalogs;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    public interface ICatalogService
    {
        Task<Result<IEnumerable<ImageCatalogModel>>> GetCatalogsAsync(string userId, CancellationToken cancellationToken);
        Task<Result<ImageCatalogModel>> GetCatalogAsync(string catalogId, CancellationToken cancellationToken);
        Task<Result<ImageCatalogModel>> CreateCatalogAsync(CreateImageCatalogModel catalogModel, CancellationToken cancellationToken);
        Task<Result<bool>> AddImageToCatalogAsync(string catalogId, ImageFileModel imageFile, CancellationToken cancellationToken);
        Task<Result<bool>> RemoveImageFromCatalogAsync(string catalogId, string[] imageIds, CancellationToken cancellationToken);
        Task<Result<bool>> SetCataglogAttributeAsync(string catalogId, SetAttribute attribute, CancellationToken cancellationToken);
        Task<Result<bool>> SetCataglogAttributesAsync(string catalogId, SetAttribute[] attributes, CancellationToken cancellationToken);
        Task<Result<bool>> RemoveCatalogAsync(string catalogId, CancellationToken cancellationToken);
    }
}
