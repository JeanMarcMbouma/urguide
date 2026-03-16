using BbQ.Cqrs;
using Microsoft.EntityFrameworkCore;
using BbQ.Outcome;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Data;
using UrGuide.Data.Entities.Posts;
using UrGuide.Model;
using UrGuide.Model.Posts;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;
using UrGuide.Services.Abstraction;
using UrGuide.Services.Auditing.Command;
using UrGuide.Services.Contracts;
using UrGuide.Services.Extensions;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Posts
{
    class PostService : BaseService, IPostService, IBidService
    {
        public PostService(UrGuideContext context,
                           IUserContext userContext,
                           ICatalogService catalogService,
                           IIPStackService iPStackService,
                           ILogger<PostService> logger,
                           IEmailService emailService,
                           IImageService imageService,
                           IUserNotificationService notificationService,
                           IMediator mediator,
                           IElasticsearchService elasticsearchService) : base(context, userContext)
        {
            CatalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            IPStackService = iPStackService ?? throw new ArgumentNullException(nameof(iPStackService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            EmailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            ElasticsearchService = elasticsearchService ?? throw new ArgumentNullException(nameof(elasticsearchService));
        }

        public ICatalogService CatalogService { get; }
        public IIPStackService IPStackService { get; }
        public ILogger<PostService> Logger { get; }
        public IEmailService EmailService { get; }
        public IImageService ImageService { get; }
        public IUserNotificationService NotificationService { get; }
        public IMediator Mediator { get; }
        public IElasticsearchService ElasticsearchService { get; }

        public async Task<Outcome<PostModel>> AcceptBidAsync(string postId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();
            var post = await Context.Posts
                .Include(x => x.Bid)
                .ThenInclude(x => x.Author).FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
            if (post == null)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            try
            {
                post.AcceptBid();
                var author = post.Bid.Author;
                var authorFirstName = author.FirstName;
                var authorEmail = author.Email;
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
                return Result.Of(PostMapper.ToPostModel(PostVisitor.Visit(post, UserContext.UserId)));
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

        public async Task<Outcome<PostModel>> CreatePostAsync(PostCreationModel model, CancellationToken cancellationToken)
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
            if (result.IsError)
                return Result.Of<PostModel>().WithErrors("Failed to create a post").Combine(result);

            post.Tags = string.Join(",",model.Categories);
            post.AllocatedSeats = model.Seats;
            post.ReservedSeats = 0;
            post.StartDate = model.StartDate;
            post.EndDate = model.EndDate;
            post.BidEnabled = model.BidOptIn;
            post.Cost = model.UnitPrice;
            post.GeoLocation = model.GeoLocation;
            post.ItineraryCount = model.Itineraries.Count();

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
            await Mediator.Send(new PostCreatedCommand(UserContext.UserId, post.Id), cancellationToken);
            foreach (var image in post.Catalog.Images)
            {
                ImageService.SaveImage(image);
            }
            
            await Context.SaveChangesAsync(cancellationToken);
            
            // Index to Elasticsearch asynchronously
            try
            {
                var searchDoc = Search.SearchDocumentMapper.ToSearchDocument(post);
                _ = ElasticsearchService.IndexPostAsync(searchDoc, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to index post {PostId} to Elasticsearch", post.Id);
            }
            
            return Result.Of(PostMapper.ToPostModel(PostVisitor.Visit(post, UserContext.UserId)));
        }

        public async Task<Outcome<bool>> DeletePostAsync(string id, CancellationToken cancellationToken)
        {
            var post = await Context.Posts.Where(x => x.User.Id == UserContext.UserId && x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            if (post == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);
            var images = post.Catalog.Images;
            Context.Posts.Remove(post);
            await Context.SaveChangesAsync(cancellationToken);
            ImageService.DeleteImages(images);
            await Mediator.Send(new PostDeletedCommand(UserContext.UserId, id), cancellationToken);
            
            // Delete from Elasticsearch asynchronously
            try
            {
                _ = ElasticsearchService.DeletePostAsync(id, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to delete post {PostId} from Elasticsearch", id);
            }
            
            return Result.Of(true);
        }

        public async Task<Outcome<IEnumerable<ItineraryModel>>> GetItinerariesAsync(string postId, CancellationToken cancellationToken)
        {
            var post = await Context.Posts.Include(x => x.Itineraries)
                .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
            if (post == null)
                return Result.Of<IEnumerable<ItineraryModel>>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            return Result.Of(post.Itineraries.Select(PostMapper.ToItineraryModel).AsEnumerable());
        }

        public Task<Outcome<IEnumerable<PostModel>>> GetLast10PostsAsync(CancellationToken cancellationToken)
        {
            return GetPagedData(0, 10, cancellationToken);
        }

        private async Task<Outcome<IEnumerable<PostModel>>> GetPagedData(int offset, int size, CancellationToken cancellationToken)
        {
            var geo = await IPStackService.GetLocationAsync(UserContext);
            var posts = await Context.Posts
                .Where(x => geo == null || x.Location == null || x.Location.Distance(geo) <= Constants.Distance)
                .OrderByDescending(x => x.Id)
                .Skip(offset)
                .Take(size)
                .ToListAsync(cancellationToken);

            return Result.Of(PostVisitor.Visit(posts, UserContext.UserId).Select(PostMapper.ToPostModel));
        }

        public Task<Outcome<IEnumerable<PostModel>>> GetLast100PostsAsync(CancellationToken cancellationToken)
        {
            return GetPagedData(0, 100, cancellationToken);
        }


        public async Task<Outcome<PostModel>> OpenBidAsync(BidModel model, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();
            var post = await Context.Posts
                .Include(x => x.Bid)
                .Include(x => x.User)
                .ThenInclude(user => user.Attributes)
                .FirstOrDefaultAsync(x => x.Id == model.PostId, cancellationToken);
            if (post == null)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotFoundEntityForKey);

            if (!post.BidEnabled)
            {
                return Result.Of<PostModel>().WithErrors("This post is not biddable.");
            }

            try
            {
                var user = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
                var oldBid = post.Bid.Author?.Id;

                post.NewBid(model.Value, user);
                var author = post.User;
                var authorFirstName = author.FirstName;
                var authorEmail = author.Email;
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
                return Result.Of(PostMapper.ToPostModel(PostVisitor.Visit(post, UserContext.UserId)));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Cannot accept the current bid. State corrupted: {0}", model.PostId);
                return Result.Of<PostModel>().WithErrors(e.Message);
            }
        }

        public async Task<Outcome<PostModel>> RejectBidAsync(string postId, CancellationToken cancellationToken)
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
                var author = post.Bid.Author;
                var value = post.Bid.NewValue;
                post.RejectBid();
                var authorFirstName = author.FirstName;
                var authorEmail = author.LastName;
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
                return Result.Of(PostMapper.ToPostModel(PostVisitor.Visit(post, UserContext.UserId)));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Cannot reject the current bid. State corrupted: {0}", postId);
                return Result.Of<PostModel>().WithErrors(e.Message);
            }
        }

        public async Task<Outcome<bool>> UpdatePostAsync(PostUpdateModel model, CancellationToken cancellationToken)
        {
            var post = await Context.Posts.Where(x => x.User.Id == UserContext.UserId && x.Id == model.Id)
               .FirstOrDefaultAsync(cancellationToken);
            if (post == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);
            post.Text = model.Text;
            post.Description = model.Description;
            post.LastUpdated = DateTime.UtcNow;
            await Mediator.Send(new PostEditedCommand(UserContext.UserId, model.Id), cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            
            // Update in Elasticsearch asynchronously
            try
            {
                var searchDoc = Search.SearchDocumentMapper.ToSearchDocument(post);
                _ = ElasticsearchService.UpdatePostAsync(searchDoc, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to update post {PostId} in Elasticsearch", model.Id);
            }
            
            return Result.Of(true);
        }

        public async Task<Outcome<IEnumerable<BidHistoryModel>>> GetBidHistoryAsync(string postId, CancellationToken cancellationToken)
        {
            var post = await Context.Posts.Include(p => p.BidHistories).Where(x => x.Id == postId)
               .FirstOrDefaultAsync(cancellationToken);
            if (post == null)
                return Result.Of<IEnumerable<BidHistoryModel>>().WithErrors(ErrorMessages.NotFoundEntityForKey);

            return Result.Of(post.BidHistories.OrderByDescending(x => x.Created).Select(PostMapper.ToBidHistoryModel).AsEnumerable());
        }

        public Task<Outcome<IEnumerable<PostModel>>> GetTop10PostsAsync(CancellationToken cancellationToken)
        {
            return GetTopPagedData(0, 10, cancellationToken);
        }

        public Task<Outcome<IEnumerable<PostModel>>> GetTop100PostsAsync(CancellationToken cancellationToken)
        {
            return GetTopPagedData(0, 100, cancellationToken);
        }

        private async Task<Outcome<IEnumerable<PostModel>>> GetTopPagedData(int offset, int size, CancellationToken cancellationToken)
        {
            var geo = await IPStackService.GetLocationAsync(UserContext);
            var posts = await Context.Posts
                .Where(x => geo == null || x.Location == null || x.Location.Distance(geo) <= Constants.Distance)
                .OrderByDescending(x => x.Rating)
                .ThenBy(x => x.EndDate)
                .Skip(offset)
                .Take(size)
                .ToListAsync(cancellationToken);

            return Result.Of(PostVisitor.Visit(posts, UserContext.UserId).Select(PostMapper.ToPostModel));
        }

        public async Task<Outcome<bool>> ReserveSeatsAsync(SeatReservationModel seatReservation, CancellationToken cancellationToken)
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
                var author = post.User;
                var authorFirstName = author.FirstName;
                var authorEmail = author.Email;
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

        public async Task<Outcome<bool>> UpdateSeatReservationAsync(SeatReservationModel seatReservation, CancellationToken cancellationToken)
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
                var author = post.User;
                var authorFirstName = author.FirstName;
                var authorEmail = author.Email;
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

        public async Task<Outcome<bool>> CancelReservationAsync(string postId, CancellationToken cancellationToken)
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

                var author = post.User;
                var authorFirstName = author.FirstName;
                var authorEmail = author.Email;
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

        public async Task<Outcome<bool>> RecordUserReactionAsync(UserReactionModel userReaction, CancellationToken cancellationToken)
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

            var author = post.User;
            var authorFirstName = author.FirstName;
            var authorEmail = author.Email;
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

        public async Task<Outcome<PostModel>> GetByIdAsync(string postId, CancellationToken cancellationToken)
        {
            var post = await Context.Posts
                .Include(x => x.Bid)
                .ThenInclude(bid => bid.Author)
                .ThenInclude(author => author.Attributes)
                .FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);

            if (post == null)
                return Result.Of<PostModel>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            return Result.Of(PostMapper.ToPostModel(PostVisitor.Visit(post, UserContext.UserId)));
        }

        public Task<Outcome<PagedList<PostModel>>> GetOwnPostsAsync(SearchParameters pagination, CancellationToken cancellationToken)
        {
            return GetPostsByUserId(UserContext.UserId, pagination, cancellationToken);
        }

        public Task<Outcome<PagedList<PostModel>>> GetPostsByUserId(string userId, SearchParameters pagination, CancellationToken cancellationToken)
        {
            return InternalSearch(userId, pagination, cancellationToken);
        }

        private async Task<Outcome<PagedList<PostModel>>> InternalSearch(string? userId, SearchParameters pagination, CancellationToken cancellationToken)
        {
            var geo = pagination.Nearby ? await IPStackService.GetLocationAsync(UserContext) : null;
            var where = pagination.Extra.Any() ? $"WHERE ({string.Join(" OR ", pagination.Extra.Select(x => $"Tags LIKE '%{x}%'"))})" : string.Empty;
            var query = $"SELECT * FROM ug.Posts {where}";
            var posts = await PagedList.Of(Context.Posts.FromSqlRaw(query)
                            .Where(x => pagination.Term == null || 
                            ( 
                                EF.Functions.Like(x.GeoLocation, $"%{pagination.Term}%") || 
                                EF.Functions.Like(x.Tags, $"%{pagination.Term}%"))
                            )
                            .Where(x => userId == null || x.User == null || x.User.Id == userId)
                            .Where(x => geo == null || x.Location.Distance(geo) <= Constants.Distance)
                            .OrderByDescending(x => x.Id), pagination.PageNumber, cancellationToken);

            var pagedResult = posts.To(p => PostMapper.ToPostModel(PostVisitor.Visit(p, UserContext.UserId)));

            return Result.Of(pagedResult);
        }

        public Task<Outcome<PagedList<PostModel>>> GetPostsAsync(SearchParameters pagination, CancellationToken cancellationToken)
        {
            return InternalSearch(null, pagination, cancellationToken);
        }
    }
}
