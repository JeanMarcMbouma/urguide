using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Data.Entities.Posts;
using UrGuide.Model;
using UrGuide.Model.Posts;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;
using UrGuide.Services.Abstraction;
using UrGuide.Services.Contracts;
using UrGuide.Services.Extensions;
using UrGuide.Services.Helpers;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Posts
{
    class PostService : BaseService, IPostService, IBidService
    {
        public PostService(UrGuideContext context,
                           IUserContext userContext,
                           ICatalogService catalogService,
                           IMapper mapper,
                           IIPStackService iPStackService,
                           ILogger<PostService> logger) : base(context, userContext)
        {
            CatalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            IPStackService = iPStackService ?? throw new ArgumentNullException(nameof(iPStackService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ICatalogService CatalogService { get; }
        public IMapper Mapper { get; }
        public IIPStackService IPStackService { get; }
        public ILogger<PostService> Logger { get; }

        public async Task<Result<PostModel>> AcceptBidAsync(string postId, CancellationToken cancellationToken)
        {
            if (UserContext.IsAuthenticated)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();
            var post = await Context.Posts
                .Include(x => x.Bid)
                .ThenInclude(x => x.Author)
                .Include(x => x.BidHistories)
                .ThenInclude(x => x.Author)
                .Include(x => x.Attributes).FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
            if (post == null)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            try
            {
                post.AcceptBid();
                return Result.Of(Mapper.Map<PostModel>(post));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Cannot accept the current bid. State corrupted: {0}", postId);
                return Result.Of<PostModel>().WithErrors(e.Message);
            }
        }

        public async Task<Result<PostModel>> CreatePostAsync(PostCreationModel model, CancellationToken cancellationToken)
        {
            var user = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
            var post = new Post
            {
                User = user,
                Text = model.Text,
                DateOfPublication = DateTime.UtcNow,
                Description = model.Description,
                LastUpdated = DateTime.UtcNow
            };

            await post.SetLocationAsync(UserContext, IPStackService);

            var extFiles = new List<ImageFileCreateModel>();
            if(model.Video != null)
            {
                extFiles.Add(model.Video);
            }

            var result = await CatalogService.AddCatalogToPostAsync(post, new Model.Catalogs.CreateImageCatalogModel
            {
                Description = model.Description,
                Name = model.Description,
                Files = model.Images.Union(extFiles).ToList()
            }, cancellationToken);
            if (result.HasError)
                return Result.Of<PostModel>().WithErrors("Failed to create a post").Combine(result);

            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.Dislikes), Value = Constants.Zero });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.Likes), Value = Constants.Zero });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.AllocatedSeats), Value = model.Seats.ToString() });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.Categories), Value = string.Join(",", model.Categories) });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.Amount), Value = model.UnitPrice });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.DateStart), Value = DateTimeHelper.GetDate(model.StartDate) });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.TimeStart), Value = DateTimeHelper.GetTime(model.StartDate) });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.DateEnd), Value = DateTimeHelper.GetDate(model.EndDate) });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.TimeEnd), Value = DateTimeHelper.GetTime(model.EndDate) });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.GeoLocation), Value = model.GeoLocation });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.Views), Value = Constants.Zero });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.PublicationDate), Value = DateTimeHelper.GetDateTime(post.DateOfPublication) });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.Status), Value = Constants.Active });


            Context.Posts.Add(post);
            await Context.SaveChangesAsync(cancellationToken); 
            return Result.Of(Mapper.Map<PostModel>(post));
        }

        public async Task<Result<bool>> DeletePostAsync(string id, CancellationToken cancellationToken)
        {
            var post = await Context.Posts.Where(x => x.User.Id == UserContext.UserId && x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            if (post == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);
            Context.Posts.Remove(post);
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }

        public async Task<Result<IEnumerable<PostModel>>> GetLast10PostsAsync(CancellationToken cancellationToken)
        {
            var geo = await IPStackService.GetLocationAsync(UserContext);

            var posts = await Context.Posts.Include(x => x.Attributes)
                .Include(x => x.Catalog)
                .ThenInclude(x => x.Images)
                .ThenInclude(x => x.Attributes)
                .Where(x => x.Location == null || geo == null || x.Location.Distance(geo) <= Constants.Distance)
                .OrderByDescending(x => x.LastUpdated)
                .Take(10).AsNoTracking().ToListAsync(cancellationToken);
            return Result.Of(Mapper.Map<IEnumerable<PostModel>>(posts));
        }

        public async Task<Result<PostModel>> OpenBidAsync(BidModel model, CancellationToken cancellationToken)
        {
            if (UserContext.IsAuthenticated)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();
            var post = await Context.Posts
                .Include(x => x.Bid)
                .ThenInclude(x => x.Author)
                .Include(x => x.BidHistories)
                .ThenInclude(x => x.Author)
                .Include(x => x.Attributes).FirstOrDefaultAsync(x => x.Id == model.PostId, cancellationToken);
            if (post == null)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            try
            {
                var user = await Context.Users.FindAsync(new { UserContext.UserId }, cancellationToken);

                post.NewBid(model.Value, user);
                return Result.Of(Mapper.Map<PostModel>(post));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Cannot accept the current bid. State corrupted: {0}", model.PostId);
                return Result.Of<PostModel>().WithErrors(e.Message);
            }
        }

        public async Task<Result<PostModel>> RejectBidAsync(string postId, CancellationToken cancellationToken)
        {
            if (UserContext.IsAuthenticated)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();
            var post = await Context.Posts
                .Include(x => x.Bid)
                .ThenInclude(x => x.Author)
                .Include(x => x.BidHistories)
                .ThenInclude(x => x.Author)
                .Include(x => x.Attributes).FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
            if (post == null)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            try
            {
                post.RejectBid();
                return Result.Of(Mapper.Map<PostModel>(post));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Cannot reject the current bid. State corrupted: {0}", postId);
                return Result.Of<PostModel>().WithErrors(e.Message);
            }
        }

        public async Task<Result<bool>> UpdatePostAsync(PostUpdateModel model, CancellationToken cancellationToken)
        {
            var post = await Context.Posts.Where(x => x.User.Id == UserContext.UserId && x.Id == model.Id)
               .FirstOrDefaultAsync(cancellationToken);
            if (post == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);
            post.Text = model.Text;
            post.Description = model.Description;
            post.LastUpdated = DateTime.UtcNow;
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }

        public Task<Result<bool>> UpdatePostAttributesAsync(string id, SetAttribute[] attributes, CancellationToken cancellationToken)
        {
            return SetAttributesRestrictedToUserAsync<Post>(id, attributes, cancellationToken);
        }
    }
}
