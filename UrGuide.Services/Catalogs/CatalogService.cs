using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Data.Entities.Shared;
using UrGuide.Model;
using UrGuide.Model.Catalogs;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;
using UrGuide.Services.Abstraction;
using UrGuide.Services.Contracts;
using UrGuide.Services.Helpers;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Catalogs
{
    class CatalogService : BaseService, ICatalogService
    {
        public CatalogService(IUserContext userContext, UrGuideContext context, IMapper mapper) : base(context, userContext)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IMapper Mapper { get; }

        public async Task<Result<bool>> AddCatalogToPostAsync(Data.Entities.Posts.Post post, CreateImageCatalogModel catalogModel, CancellationToken cancellationToken)
        {
            if (post is null)
            {
                return Result.Of(false).WithErrors("No post was given");
            }

            var result = await CreateCatalogInternal(catalogModel, cancellationToken);
            if (result.HasError)
                return Result.Of(false).Combine(result).WithErrors("Failed to add a catalog to post");
            post.Catalog = result.Data;
            return Result.Of(true);
        }

        public async Task<Result<bool>> AddImageToCatalogAsync(string catalogId, ImageFileCreateModel imageFile, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var catalog = await Context.ImageCatalogs.Include(x => x.Images)
                .Include(x => x.User)
                .Where(x => x.User.Id == UserContext.UserId)
                .FirstOrDefaultAsync(x => x.Id == catalogId, cancellationToken);
            if (catalog == null)
                return Result.Of(false).WithErrors("Catalog doesn't exists");
            catalog.Images.Add(new Image
            {
                ImageBase64 = imageFile.ImageBase64,
                MimeType = FileExtensionHelper.GetImageMimeType(imageFile)
            });

            catalog.LastUpdated = DateTime.UtcNow;
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }

        public async Task<Result<ImageCatalogModel>> CreateCatalogAsync(CreateImageCatalogModel catalogModel, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<ImageCatalogModel>().WithErrors(ErrorMessages.NotAuthenticated);
            cancellationToken.ThrowIfCancellationRequested();
            var catalog = await CreateCatalogInternal(catalogModel, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(Mapper.Map<ImageCatalogModel>(catalog.Data));
        }

        private async Task<Result<ImageCatalog>> CreateCatalogInternal(CreateImageCatalogModel catalogModel, CancellationToken cancellationToken)
        {
            var user = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
            var catalog = new ImageCatalog
            {
                Created = DateTime.UtcNow,
                User = user,
                LastUpdated = DateTime.UtcNow
            };

            foreach (var file in catalogModel.Files)
            {
                var image = new Image
                {
                    ImageBase64 = file.ImageBase64,
                    MimeType = FileExtensionHelper.GetImageMimeType(file)
                };
                image.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(file.Name), Value = file.Name });
                catalog.Images.Add(image);
            }
            catalog.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(catalogModel.Name), Value = catalogModel.Name });
            catalog.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(catalogModel.Description), Value = catalogModel.Description });
            Context.ImageCatalogs.Add(catalog);
            return Result.Of(catalog);
        }

        public async Task<Result<ImageCatalogModel>> GetCatalogAsync(string catalogId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var catalog = await Context.ImageCatalogs.FindAsync(new[] { catalogId }, cancellationToken);
            if (catalog == null)
                return Result.Of<ImageCatalogModel>().WithErrors("Catalog doesn't exists");
            return Result.Of(Mapper.Map<ImageCatalogModel>(catalog));
        }

        public async Task<Result<IEnumerable<ImageCatalogModel>>> GetCatalogsAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var catalogs = await Context.ImageCatalogs.Include(c => c.User).Where(x => x.User.Id == userId)
                .Select(catalog => Mapper.Map<ImageCatalogModel>(catalog))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return Result.Of(catalogs.AsEnumerable());
        }

        public async Task<Result<bool>> RemoveCatalogAsync(string catalogId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var catalog = await Context.ImageCatalogs
                    .Include(x => x.User)
                    .Where(x => x.User.Id == UserContext.UserId && x.Id == catalogId).FirstOrDefaultAsync(cancellationToken);
            if (catalog == null)
                return Result.Of(false).WithErrors("Catalog doesn't exists");

            Context.ImageCatalogs.Remove(catalog);

            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }

        public async Task<Result<bool>> RemoveImageFromCatalogAsync(string catalogId, string[] imageIds, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var catalog = await Context.ImageCatalogs.Include(x => x.Images)
                .Include(x => x.User)
                .Where(x => x.User.Id == UserContext.UserId)
                .FirstOrDefaultAsync(x => x.Id == catalogId, cancellationToken);
            if (catalog == null)
                return Result.Of(false).WithErrors("Catalog doesn't exists");
            var images = catalog.Images.Where(i => imageIds.Any(v => v.Equals(i.Id))).ToList();
            images.ForEach(i => catalog.Images.Remove(i));
            catalog.LastUpdated = DateTime.UtcNow;
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }


        public Task<Result<bool>> SetCataglogAttributesAsync(string catalogId, SetAttribute[] attributes, CancellationToken cancellationToken)
        {
            return SetAttributesRestrictedToUserAsync<ImageCatalog>(catalogId, attributes, cancellationToken);
        }
    }
}
