using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Data.Entities.Posts;
using UrGuide.Data.Shared;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;
using UrGuide.Services.Abstraction;
using UrGuide.Services.Contracts;
using UrGuide.Services.Extensions;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Feedback
{
    class FeedbackService : BaseService, IFeedbackService
    {
        public FeedbackService(UrGuideContext context,
                               IEmailService emailService,
                               IUserContext userContext,
                               IMapper mapper,
                               IUserNotificationService notificationService)
            : base(context, userContext)
        {
            EmailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public IEmailService EmailService { get; }
        public IMapper Mapper { get; }
        public IUserNotificationService NotificationService { get; }

        public async Task<Result<bool>> AddPostFeedbackAsync(string postId, FeedbackModel feedback, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var post = await Context.Posts.FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
            if (post == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);
            if(post.User.Id == UserContext.UserId)
            {
                return Result.Of(false).WithErrors("You cannot write a review against your own post");
            }

            var rating = post.Attributes.GetItem(Data.Entities.Posts.AttributeTypes.Rating);
            if(rating == null)
            {
                rating = new Data.Entities.Attributes.GenericAttribute
                {
                    Name = nameof(Data.Entities.Posts.AttributeTypes.Rating),
                    Value = Constants.Zero
                };
                post.Attributes.Add(rating);
            }
            var reviews = post.Attributes.GetItem(Data.Entities.Posts.AttributeTypes.Reviews);
            if(reviews == null)
            {
                reviews = new Data.Entities.Attributes.GenericAttribute
                {
                    Name = nameof(Data.Entities.Posts.AttributeTypes.Reviews),
                    Value = Constants.Zero
                };
                post.Attributes.Add(reviews);
            }

            int reviewCount = reviews;
            reviews.Value = (reviewCount + 1).ToString();
            int r = rating;
            int avg = reviewCount == 0 ? feedback.Rating : (int)Math.Ceiling(new[] { r, feedback.Rating }.Average());
            rating.Value = r.ToString();
            var author = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
            string authorFirstName = author.Attributes.Get<string>(Data.Entities.Users.AttributeTypes.FirstName);

            string postAuthorFirstName = post.User.Attributes.Get<string>(Data.Entities.Users.AttributeTypes.FirstName);
            string postAuthorEmail = post.User.Attributes.Get<string>(Data.Entities.Users.AttributeTypes.EmailAddress);
            
            post.Feedback.Add(new Data.Shared.Feedback
            {
                Author = author,
                Rating = feedback.Rating,
                Text = feedback.Text
            });

            string content = $@"
Hi, {postAuthorFirstName}!
You've got a new feedback from {authorFirstName}

{feedback.Text}
...
Rating: {feedback.Rating} star(s).";
            await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
            {
                Content = content,
                Subject = $"{post.Text} - New feedback",
                ToName = postAuthorFirstName,
                To = postAuthorEmail
            }) ;
            await NotificationService.SystemNotifyAsync(author.Id, content, null);
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }

        public async Task<Result<bool>> AddUserFeedbackAsync(string userId, FeedbackModel feedback, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);
            if (user.Id == UserContext.UserId)
            {
                return Result.Of(false).WithErrors("You cannot write a review against your own post");
            }

            var rating = user.Attributes.GetItem(Data.Entities.Users.AttributeTypes.Rating);
            var firstRatingEver = false;
            if (rating == null)
            {
                firstRatingEver = true;
                rating = new Data.Entities.Attributes.GenericAttribute
                {
                    Name = nameof(Data.Entities.Posts.AttributeTypes.Rating),
                    Value = Constants.Zero
                };
                user.Attributes.Add(rating);
            }

            int r = rating;
            int avg = firstRatingEver ? feedback.Rating : (int)Math.Ceiling(new[] { r, feedback.Rating }.Average());
            rating.Value = r.ToString();
            var author = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
            string authorFirstName = author.Attributes.Get<string>(Data.Entities.Users.AttributeTypes.FirstName);

            string userFirstName = user.Attributes.Get<string>(Data.Entities.Users.AttributeTypes.FirstName);
            string userEmail = user.Attributes.Get<string>(Data.Entities.Users.AttributeTypes.EmailAddress);

            user.Feedback.Add(new Data.Shared.Feedback
            {
                Author = author,
                Rating = feedback.Rating,
                Text = feedback.Text
            });

            string content = $@"
Hi, {userFirstName}!
You've got a new feedback from {authorFirstName}

{feedback.Text}
...
Rating: {feedback.Rating} star(s).";
            await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
            {
                Content = content,
                Subject = "New feedback",
                ToName = userFirstName,
                To = userEmail
            });
            await NotificationService.SystemNotifyAsync(user.Id, content, null);
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }

        public async Task<Result<PagedList<AuthoredFeedback>>> GetPostFeedback(string postId, PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            var post = await Context.Posts.FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
            if (post == null)
                return Result.Of<PagedList<AuthoredFeedback>>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            return Result.Of(PagedList.Of(post.Feedback.OrderByDescending(f => f.Created).AsEnumerable(),
                paginationParameters.PageNumber, f => Mapper.Map<AuthoredFeedback>(f)));
        }

        public async Task<Result<PagedList<AuthoredFeedback>>> GetUserFeedback(string userId, PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            var user = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user == null)
               return Result.Of<PagedList<AuthoredFeedback>>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            return Result.Of(PagedList.Of(user.Feedback.OrderByDescending(f => f.Created).AsEnumerable(),
                paginationParameters.PageNumber, f => Mapper.Map<AuthoredFeedback>(f)));
        }
    }
}
