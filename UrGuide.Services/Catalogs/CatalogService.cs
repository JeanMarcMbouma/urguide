using BbQ.Cqrs;
using Microsoft.EntityFrameworkCore;
using BbQ.Outcome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core.Attributes;
using UrGuide.Data;
using UrGuide.Data.Entities.Shared;
using UrGuide.Model;
using UrGuide.Model.Catalogs;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;
using UrGuide.Services.Abstraction;
using UrGuide.Services.Auditing.Command;
using UrGuide.Services.Contracts;
using UrGuide.Services.Extensions;
using UrGuide.Services.Helpers;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Catalogs
{
    class CatalogService : BaseService, ICatalogService
    {
        public CatalogService(
            IUserContext userContext,
            UrGuideContext context,
            IIPStackService iPStackService,
            IImageService imageService,
            IMediator mediator) : base(context, userContext)
        {
            IPStackService = iPStackService ?? throw new ArgumentNullException(nameof(iPStackService));
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public IIPStackService IPStackService { get; }
        public IImageService ImageService { get; }
        public IMediator Mediator { get; }

        public async Task<Outcome<bool>> AddCatalogToPostAsync(Data.Entities.Posts.Post post, CreateImageCatalogModel catalogModel, CancellationToken cancellationToken)
        {
            if (post is null)
            {
                return Result.Of(false).WithErrors("No post was given");
            }

            var result = await CreateCatalogInternal(catalogModel, cancellationToken);
            if (result.IsError)
                return Result.Of(false).Combine(result).WithErrors("Failed to add a catalog to post");
            post.Catalog = result.Value;
            return Result.Of(true);
        }

        public async Task<Outcome<ImageFileModel>> AddImageToCatalogAsync(string catalogId, ImageFileCreateModel imageFile, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<ImageFileModel>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var catalog = await Context.ImageCatalogs.Include(x => x.Images)
                .Include(x => x.User)
                .Where(x => x.User.Id == UserContext.UserId)
                .FirstOrDefaultAsync(x => x.Id == catalogId, cancellationToken);
            if (catalog == null)
                return Result.Of<ImageFileModel>().WithErrors("Catalog doesn't exists");

            var newImage = new Image
            {
                ImageUrl = imageFile.ImageBase64,
                MimeType = FileExtensionHelper.GetImageMimeType(imageFile)
            };
            newImage.Attributes.Add(new GenericAttribute { Name = nameof(imageFile.Name), Value = imageFile.Name });

            catalog.Images.Add(newImage);

            catalog.LastUpdated = DateTime.UtcNow;
            await Context.SaveChangesAsync(cancellationToken);

            ImageService.SaveImage(newImage);

            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(CatalogMapper.ToImageFileModel(newImage));
        }

        public async Task<Outcome<ImageCatalogModel>> CreateCatalogAsync(CreateImageCatalogModel catalogModel, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<ImageCatalogModel>().WithErrors(ErrorMessages.NotAuthenticated);
            cancellationToken.ThrowIfCancellationRequested();
            var catalog = await CreateCatalogInternal(catalogModel, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            await Mediator.Send(new CatalogCreatedCommand(UserContext.UserId, catalog.Value.Id), cancellationToken);
            return Result.Of(CatalogMapper.ToImageCatalogModel(catalog.Value));
        }

        private async Task<Outcome<ImageCatalog>> CreateCatalogInternal(CreateImageCatalogModel catalogModel, CancellationToken cancellationToken)
        {
            var user = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
            var catalog = new ImageCatalog
            {
                Created = DateTime.UtcNow,
                User = user,
                LastUpdated = DateTime.UtcNow
            };

            await catalog.SetLocationAsync(UserContext, IPStackService);

            foreach (var file in catalogModel.Files)
            {
                var image = new Image
                {
                    ImageUrl = file.ImageBase64,
                    MimeType = FileExtensionHelper.GetImageMimeType(file)
                };
                image.Attributes.Add(new GenericAttribute { Name = nameof(file.Name), Value = file.Name });
                catalog.Images.Add(image);
            }
            catalog.Attributes.Add(new GenericAttribute { Name = nameof(catalogModel.Name), Value = catalogModel.Name });
            catalog.Attributes.Add(new GenericAttribute { Name = nameof(catalogModel.Description), Value = catalogModel.Description });
            Context.ImageCatalogs.Add(catalog);
            return Result.Of(catalog);
        }

        public async Task<Outcome<ImageCatalogModel>> GetCatalogAsync(string catalogId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var catalog = await Context.ImageCatalogs.FindAsync(new[] { catalogId }, cancellationToken);
            if (catalog == null)
                return Result.Of<ImageCatalogModel>().WithErrors("Catalog doesn't exists");
            return Result.Of(CatalogMapper.ToImageCatalogModel(catalog));
        }

        public async Task<Outcome<IEnumerable<ImageCatalogModel>>> GetCatalogsAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var catalogIds = await Context.ImageCatalogs.FromSqlInterpolated($"SELECT Image_CatalogId FROM ug.Image_Catalogs WHERE UserId = {userId}").Select( x => x.Id)
                .ToListAsync(cancellationToken);
            var catalogs = await Context.ImageCatalogs.Where(x => catalogIds.Contains(x.Id)).ToListAsync(cancellationToken);
            return Result.Of(catalogs.Select(catalog => CatalogMapper.ToImageCatalogModel(catalog)).AsEnumerable());
        }

        public async Task<Outcome<IEnumerable<ImageCatalogModel>>> GetCatalogsAsync(CancellationToken cancellationToken)
        {
            var geo = await IPStackService.GetLocationAsync(UserContext);

            cancellationToken.ThrowIfCancellationRequested();
            var catalogs = await Context.ImageCatalogs
                .Where(x => x.Location == null || geo == null || x.Location.Distance(geo) <= Constants.Distance)
                .ToListAsync(cancellationToken);
            return Result.Of(catalogs.Select(catalog => CatalogMapper.ToImageCatalogModel(catalog)).AsEnumerable());
        }

        public async Task<Outcome<bool>> RemoveCatalogAsync(string catalogId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var catalog = await Context.ImageCatalogs
                    .Include(x => x.User)
                    .Where(x => x.User.Id == UserContext.UserId && x.Id == catalogId).FirstOrDefaultAsync(cancellationToken);
            if (catalog == null)
                return Result.Of(false).WithErrors("Catalog doesn't exists");
            var images = catalog.Images;
            Context.ImageCatalogs.Remove(catalog);

            await Context.SaveChangesAsync(cancellationToken);
            await Mediator.Send(new CatalogDeletedCommand(UserContext.UserId, catalogId), cancellationToken);
            ImageService.DeleteImages(images);

            return Result.Of(true);
        }

        public async Task<Outcome<bool>> RemoveImageFromCatalogAsync(string catalogId, string[] imageIds, CancellationToken cancellationToken)
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
            images.ForEach(i => {
                catalog.Images.Remove(i);
                ImageService.DeleteImage(i);
            });
            catalog.LastUpdated = DateTime.UtcNow;
            await Context.SaveChangesAsync(cancellationToken);
            await Mediator.Send(new CatalogEditedCommand(UserContext.UserId, catalog.Id), cancellationToken);
            return Result.Of(true);
        }


        public async Task<Outcome<bool>> SetCataglogAttributesAsync(string catalogId, SetAttribute[] attributes, CancellationToken cancellationToken)
        {

            await Mediator.Send(new CatalogEditedCommand(UserContext.UserId, catalogId), cancellationToken);
            return await SetAttributesRestrictedToUserAsync<ImageCatalog>(catalogId, attributes, cancellationToken);
        }
    }
}
