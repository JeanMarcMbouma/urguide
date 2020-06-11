using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
                           ILogger<PostService> logger,
                           IEmailService emailService,
                           IImageService imageService,
                           IUserNotificationService notificationService) : base(context, userContext)
        {
            CatalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            IPStackService = iPStackService ?? throw new ArgumentNullException(nameof(iPStackService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            EmailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public ICatalogService CatalogService { get; }
        public IMapper Mapper { get; }
        public IIPStackService IPStackService { get; }
        public ILogger<PostService> Logger { get; }
        public IEmailService EmailService { get; }
        public IImageService ImageService { get; }
        public IUserNotificationService NotificationService { get; }

        public async Task<Result<PostModel>> AcceptBidAsync(string postId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();
            var post = await Context.Posts
                .Include(x => x.Bid)
                .ThenInclude(x => x.Author)
                .Include(x => x.Attributes).FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
            if (post == null)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            try
            {
                post.AcceptBid();
                var author = post.Bid.Author.Attributes;
                var authorFirstName = author.Get<string>(Data.Entities.Users.AttributeTypes.FirstName);
                var authorEmail = author.Get<string>(Data.Entities.Users.AttributeTypes.EmailAddress);
                string content = @$"
Congratulation, {authorFirstName}</br>
Your bid was accepted:</br>
Post: <strong>{post.Text}</strong></br>
{post.Description}</br>
...</br>

Old price: <em>{post.Bid.OldValue}</em></br>
---------------------------------------</br>
New price: <em>{post.Bid.NewValue}</em>";

                await CreateNotification(post, post.Bid.Author.Id, content);
                await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
                {
                    Content = content,
                    Subject = "Your bid was accepted",
                    To = authorEmail,
                    ToName = authorFirstName
                });
                return Result.Of(Mapper.Map<PostModel>(PostVisitor.Visit(post, UserContext.UserId)));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Cannot accept the current bid. State corrupted: {0}", postId);
                return Result.Of<PostModel>().WithErrors(e.Message);
            }
        }

        private async Task CreateNotification(Post post, string authorId, string content)
        {
            if (!post.Catalog.Images.Any())
                return;
            await NotificationService.SystemNotifyAsync(authorId, content, $"/post/{post.Id}/shot/{post.Catalog.Images.FirstOrDefault()?.Id}");
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
            if (model.Video != null)
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
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.TimeStart), Value = DateTimeHelper.GetTime(model.StartDate, DateTimeKind.Local) });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.DateEnd), Value = DateTimeHelper.GetDate(model.EndDate) });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.TimeEnd), Value = DateTimeHelper.GetTime(model.EndDate, DateTimeKind.Local) });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.GeoLocation), Value = model.GeoLocation });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.Views), Value = Constants.Zero });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.PublicationDate), Value = DateTimeHelper.GetDateTime(post.DateOfPublication, DateTimeKind.Local) });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.Status), Value = Constants.Active });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.Rating), Value = Constants.Zero });
            post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.Reviews), Value = Constants.Zero });

            if (model.BidOptIn)
            {
                post.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(AttributeTypes.BidOptIn), Value = Constants.Yes });
            }

            foreach (var it in model.Itineraries)
            {
                post.Itineraries.Add(new Itinerary
                {
                    Description = it.Description,
                    Ordinal = it.Ordinal,
                    Title = it.Title
                });
            }

            Context.Posts.Add(post);
            await Context.SaveChangesAsync(cancellationToken);

            foreach (var image in post.Catalog.Images)
            {
                ImageService.SaveImage(image);
            }
            
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(Mapper.Map<PostModel>(PostVisitor.Visit(post, UserContext.UserId)));
        }

        public async Task<Result<bool>> DeletePostAsync(string id, CancellationToken cancellationToken)
        {
            var post = await Context.Posts.Where(x => x.User.Id == UserContext.UserId && x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            if (post == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);
            var images = post.Catalog.Images;
            Context.Posts.Remove(post);
            await Context.SaveChangesAsync(cancellationToken);
            ImageService.DeleteImages(images);
            return Result.Of(true);
        }

        public async Task<Result<IEnumerable<ItineraryModel>>> GetItinerariesAsync(string postId, CancellationToken cancellationToken)
        {
            var post = await Context.Posts.Include(x => x.Itineraries)
                .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
            if (post == null)
                return Result.Of<IEnumerable<ItineraryModel>>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            return Result.Of(Mapper.Map<IEnumerable<ItineraryModel>>(post.Itineraries));
        }

        public Task<Result<IEnumerable<PostModel>>> GetLast10PostsAsync(CancellationToken cancellationToken)
        {
            return GetPagedData(0, 10, cancellationToken);
        }

        private async Task<Result<IEnumerable<PostModel>>> GetPagedData(int offset, int size, CancellationToken cancellationToken)
        {
            var geo = await IPStackService.GetLocationAsync(UserContext);
            var p = await Context.Set<PostSearch>()
                .Where(x => geo == null || x.Location == null || x.Location.Distance(geo) <= Constants.Distance)
                .OrderByDescending(x => x.PostId)
                .Select(p => p.PostId)
                .Skip(offset)
                .Take(size)
                .ToListAsync(cancellationToken);


            var posts = await Context.Posts
                .Where(x => p.Contains(x.Id))
                .ToListAsync(cancellationToken);

            return Result.Of(Mapper.Map<IEnumerable<PostModel>>(PostVisitor.Visit(posts, UserContext.UserId)));
        }

        public Task<Result<IEnumerable<PostModel>>> GetLast100PostsAsync(CancellationToken cancellationToken)
        {
            return GetPagedData(0, 100, cancellationToken);
        }


        public async Task<Result<PostModel>> OpenBidAsync(BidModel model, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();
            var post = await Context.Posts
                .Include(x => x.Attributes)
                .Include(x => x.Bid)
                .Include(x => x.User)
                .ThenInclude(user => user.Attributes)
                .FirstOrDefaultAsync(x => x.Id == model.PostId, cancellationToken);
            if (post == null)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotFoundEntityForKey);

            if (!post.Attributes.Any(a => a.Name == nameof(AttributeTypes.BidOptIn)))
            {
                return Result.Of<PostModel>().WithErrors("This post is not biddable.");
            }

            try
            {
                var user = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
                var oldBid = post.Bid?.Author.Id;

                post.NewBid(model.Value, user);
                var author = post.User.Attributes;
                var authorFirstName = author.Get<string>(Data.Entities.Users.AttributeTypes.FirstName);
                var authorEmail = author.Get<string>(Data.Entities.Users.AttributeTypes.EmailAddress);
                string content = @$"
Hi, {authorFirstName}</br>
You received a new proposal:</br>
Post: <strong>{post.Text}</strong></br>
{post.Description}</br>
...</br>

Old price: <em>{post.Bid.OldValue}</em></br>
---------------------------------------</br>
New price: <em>{post.Bid.NewValue}</em>";
                await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
                {
                    Content = content,
                    Subject = "New proposal",
                    To = authorEmail,
                    ToName = authorFirstName
                });
                await CreateNotification(post, post.User.Id, content);
                if (!string.IsNullOrEmpty(oldBid))
                    await CreateNotification(post, oldBid, @$"
Your bid is no longer active:</br>
Post: <strong>{post.Text}</strong></br>
{post.Description}</br>
...</br>

Old price: <em>{post.Bid.OldValue}</em></br>
---------------------------------------</br>
New price: <em>{post.Bid.NewValue}</em>");
                return Result.Of(Mapper.Map<PostModel>(PostVisitor.Visit(post, UserContext.UserId)));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Cannot accept the current bid. State corrupted: {0}", model.PostId);
                return Result.Of<PostModel>().WithErrors(e.Message);
            }
        }

        public async Task<Result<PostModel>> RejectBidAsync(string postId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();
            var post = await Context.Posts
                .Include(x => x.Bid)
                .ThenInclude(bid => bid.Author)
                .ThenInclude(author => author.Attributes)
                .FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
            if (post == null)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            try
            {
                var author = post.Bid.Author.Attributes;
                var value = post.Bid.NewValue;
                post.RejectBid();
                var authorFirstName = author.Get<string>(Data.Entities.Users.AttributeTypes.FirstName);
                var authorEmail = author.Get<string>(Data.Entities.Users.AttributeTypes.EmailAddress);
                string content = @$"
Hi, {authorFirstName}</br>
Your bid was rejected by the owner:</br>
Post: <strong>{post.Text}</strong></br>
{post.Description}</br>
...</br>

Your bid: <em>{value}</em>";
                await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
                {
                    Content = content,
                    Subject = "Your bid was rejected",
                    To = authorEmail,
                    ToName = authorFirstName
                });
                await CreateNotification(post, post.Bid.Author.Id, content);
                return Result.Of(Mapper.Map<PostModel>(PostVisitor.Visit(post, UserContext.UserId)));
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

        public async Task<Result<IEnumerable<BidHistoryModel>>> GetBidHistoryAsync(string postId, CancellationToken cancellationToken)
        {
            var post = await Context.Posts.Include(p => p.BidHistories).Where(x => x.Id == postId)
               .FirstOrDefaultAsync(cancellationToken);
            if (post == null)
                return Result.Of<IEnumerable<BidHistoryModel>>().WithErrors(ErrorMessages.NotFoundEntityForKey);

            return Result.Of(Mapper.Map<IEnumerable<BidHistoryModel>>(post.BidHistories.OrderByDescending(x => x.Created)));
        }

        public Task<Result<IEnumerable<PostModel>>> GetTop10PostsAsync(CancellationToken cancellationToken)
        {
            return GetTopPagedData(0, 10, cancellationToken);
        }

        public Task<Result<IEnumerable<PostModel>>> GetTop100PostsAsync(CancellationToken cancellationToken)
        {
            return GetTopPagedData(0, 100, cancellationToken);
        }

        private async Task<Result<IEnumerable<PostModel>>> GetTopPagedData(int offset, int size, CancellationToken cancellationToken)
        {
            var geo = await IPStackService.GetLocationAsync(UserContext);
            var p = await Context.Set<PostSearch>()
                .Where(x => geo == null || x.Location == null || x.Location.Distance(geo) <= Constants.Distance)
                .OrderByDescending(x => x.Rating)
                .ThenBy(x => x.EndDate)
                .Select(p => p.PostId)
                .Skip(offset)
                .Take(size)
                .ToListAsync(cancellationToken);


            var posts = await Context.Posts
                .Where(x => p.Contains(x.Id))
                .ToListAsync(cancellationToken);

            return Result.Of(Mapper.Map<IEnumerable<PostModel>>(PostVisitor.Visit(posts, UserContext.UserId)));
        }

        public async Task<Result<bool>> ReserveSeatsAsync(SeatReservationModel seatReservation, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var post = await Context.Posts
                .Include(x => x.User)
                .ThenInclude(x => x.Attributes)
                .FirstOrDefaultAsync(p => p.Id.Equals(seatReservation.PostId), cancellationToken);
            if (post == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);
            try
            {
                post.MakeReservation(UserContext.UserId, seatReservation.Seats);
                await Context.SaveChangesAsync(cancellationToken);
                var author = post.User.Attributes;
                var authorFirstName = author.Get<string>(Data.Entities.Users.AttributeTypes.FirstName);
                var authorEmail = author.Get<string>(Data.Entities.Users.AttributeTypes.EmailAddress);
                string content = @$"
Hi, {authorFirstName}</br>
A user has just made a reservation:</br>
Post: <strong>{post.Text}</strong></br>
{post.Description}</br>
...................</br>
Seats: {seatReservation.Seats}";
                await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
                {
                    Content = content,
                    Subject = "Reservation",
                    To = authorEmail,
                    ToName = authorFirstName
                });
                await CreateNotification(post, post.User.Id, content);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error while making a reservation for PostID: {0}", seatReservation.PostId);
                return Result.Of(false).WithErrors(e.Message);
            }
            return Result.Of(true);
        }

        public async Task<Result<bool>> UpdateSeatReservationAsync(SeatReservationModel seatReservation, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var post = await Context.Posts
                .Include(x => x.User)
                .ThenInclude(x => x.Attributes)
                .FirstOrDefaultAsync(p => p.Id.Equals(seatReservation.PostId), cancellationToken);
            if (post == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);
            try
            {
                post.EditReservation(UserContext.UserId, seatReservation.Seats);
                await Context.SaveChangesAsync(cancellationToken);
                var author = post.User.Attributes;
                var authorFirstName = author.Get<string>(Data.Entities.Users.AttributeTypes.FirstName);
                var authorEmail = author.Get<string>(Data.Entities.Users.AttributeTypes.EmailAddress);
                string content = @$"
Hi, {authorFirstName}</br>
A user has changed their reservation:</br>
Post: <strong>{post.Text}</strong></br>
{post.Description}</br>
--------------------------</br>
Title: {post.Text}";
                await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
                {
                    Content = content,
                    Subject = "Reservation",
                    To = authorEmail,
                    ToName = authorFirstName
                });
                await CreateNotification(post, post.User.Id, content);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error while editing a reservation for PostID: {0}", seatReservation.PostId);
                return Result.Of(false).WithErrors(e.Message);
            }
            return Result.Of(true);
        }

        public async Task<Result<bool>> CancelReservationAsync(string postId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var post = await Context.Posts
                .Include(x => x.User)
                .ThenInclude(x => x.Attributes)
                .FirstOrDefaultAsync(p => p.Id.Equals(postId), cancellationToken);
            if (post == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);
            try
            {
                post.CancelReservation(UserContext.UserId);
                await Context.SaveChangesAsync(cancellationToken);

                var author = post.User.Attributes;
                var authorFirstName = author.Get<string>(Data.Entities.Users.AttributeTypes.FirstName);
                var authorEmail = author.Get<string>(Data.Entities.Users.AttributeTypes.EmailAddress);
                string content = @$"
Hi, {authorFirstName}</br>
A user has cancelled a reservation:</br>
Post: <strong>{post.Text}</strong></br>
{post.Description}";
                await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
                {
                    Content = content,
                    Subject = "Reservation",
                    To = authorEmail,
                    ToName = authorFirstName
                });
                await CreateNotification(post, post.User.Id, content);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error while cancelling a reservation for PostID: {0}", postId);
                return Result.Of(false).WithErrors(e.Message);
            }
            return Result.Of(true);
        }

        public async Task<Result<bool>> RecordUserReactionAsync(UserReactionModel userReaction, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var post = await Context.Posts
                .Include(x => x.User)
                .ThenInclude(x => x.Attributes)
                .FirstOrDefaultAsync(p => p.Id.Equals(userReaction.PostId), cancellationToken);
            if (post == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);

            post.RecordUserReaction(UserContext.UserId, userReaction.Like ? UserReaction.ReactionType.Like : UserReaction.ReactionType.DisLike);

            await Context.SaveChangesAsync(cancellationToken);

            var author = post.User.Attributes;
            var authorFirstName = author.Get<string>(Data.Entities.Users.AttributeTypes.FirstName);
            var authorEmail = author.Get<string>(Data.Entities.Users.AttributeTypes.EmailAddress);
            string content = @$"
Hi, {authorFirstName}</br>
A user has {(userReaction.Like ? "liked" : "reacted to")} your post:</br>
Post: <strong>{post.Text}</strong></br>
{post.Description}";
            await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
            {
                Content = content,
                Subject = "User's reaction",
                To = authorEmail,
                ToName = authorFirstName
            });
            await CreateNotification(post, post.User.Id, content);
            return Result.Of(true);
        }

        public async Task<Result<PostModel>> GetByIdAsync(string postId, CancellationToken cancellationToken)
        {
            var post = await Context.Posts
                .Include(x => x.Bid)
                .ThenInclude(bid => bid.Author)
                .ThenInclude(author => author.Attributes)
                .FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);

            if (post == null)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            return Result.Of(Mapper.Map<PostModel>(PostVisitor.Visit(post, UserContext.UserId)));
        }

        public Task<Result<PagedList<PostModel>>> GetOwnPostsAsync(SearchParameters pagination, CancellationToken cancellationToken)
        {
            return GetPostsByUserId(UserContext.UserId, pagination, cancellationToken);
        }

        public Task<Result<PagedList<PostModel>>> GetPostsByUserId(string userId, SearchParameters pagination, CancellationToken cancellationToken)
        {
            return InternalSearch(userId, pagination, cancellationToken);
        }

        private async Task<Result<PagedList<PostModel>>> InternalSearch(string userId, SearchParameters pagination, CancellationToken cancellationToken)
        {
            var geo = pagination.Nearby ? await IPStackService.GetLocationAsync(UserContext) : null; 
            var postIds = await PagedList.Of(Context.Set<PostSearch>()
                            .Where(x => pagination.Term == null || 
                            ( 
                                EF.Functions.Like(x.GeoLocation, $"%{pagination.Term}%") || 
                                EF.Functions.Like(x.Categories, $"%{pagination.Term}%"))
                            )
                            .Where(x => userId == null || x.UserId == userId)
                            .Where(x => geo == null || x.Location.Distance(geo) <= Constants.Distance)
                            .OrderByDescending(x => x.PostId)
                            .Select(p => p.PostId), pagination.PageNumber, cancellationToken);

            var pagedResult = postIds.FromIdCollection(Context.Posts
                .Include(x => x.Bid)
                .ThenInclude(bid => bid.Author)
                .ThenInclude(author => author.Attributes))
                .To(p => Mapper.Map<PostModel>(PostVisitor.Visit(p, UserContext.UserId)));

            return Result.Of(pagedResult);
        }

        public Task<Result<PagedList<PostModel>>> GetPostsAsync(SearchParameters pagination, CancellationToken cancellationToken)
        {
            return InternalSearch(null, pagination, cancellationToken);
        }
    }
}
