using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;
using UrGuide.Services.Abstraction;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Feedback
{
    class FeedbackService : BaseService, IFeedbackService
    {
        public FeedbackService(UrGuideContext context, IEmailService emailService, IUserContext userContext)
            : base(context, userContext)
        {
            EmailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public IEmailService EmailService { get; }

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

            var rating = post.Attributes.FirstOrDefault(a => a.Name.Equals(Data.Entities.Posts.AttributeTypes.Rating));
            if(rating == null)
            {
                rating = new Data.Entities.Attributes.GenericAttribute
                {
                    Name = nameof(Data.Entities.Posts.AttributeTypes.Rating),
                    Value = Constants.Zero
                };
                post.Attributes.Add(rating);
            }
            var reviews = post.Attributes.FirstOrDefault(a => a.Name.Equals(Data.Entities.Posts.AttributeTypes.Reviews));
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
            int avg = (int)Math.Ceiling(new[] { r, feedback.Rating }.Average());
            rating.Value = r.ToString();
            var author = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
            string authorFirstName = author.Attributes.First(x => x.Name == nameof(Data.Entities.Users.AttributeTypes.FirstName));

            string postAuthorFirstName = post.User.Attributes.First(x => x.Name == nameof(Data.Entities.Users.AttributeTypes.FirstName));
            string postAuthorEmail = post.User.Attributes.First(x => x.Name == nameof(Data.Entities.Users.AttributeTypes.EmailAddress));
            
            post.Feedback.Add(new Data.Shared.Feedback
            {
                Author = author,
                Rating = feedback.Rating,
                Text = feedback.Text
            });

            await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
            {
                Content = $@"
Hi, {postAuthorFirstName}!
You've got a new feedback from {authorFirstName}

{feedback.Text}
...
Rating: {feedback.Rating} star(s).",
                Subject = $"{post.Text} - New feedback",
                ToName = postAuthorFirstName,
                To = postAuthorEmail
            }) ;
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

            var rating = user.Attributes.FirstOrDefault(a => a.Name.Equals(Data.Entities.Users.AttributeTypes.Rating));
            if (rating == null)
            {
                rating = new Data.Entities.Attributes.GenericAttribute
                {
                    Name = nameof(Data.Entities.Posts.AttributeTypes.Rating),
                    Value = Constants.Zero
                };
                user.Attributes.Add(rating);
            }

            int r = rating;
            int avg = (int)Math.Ceiling(new[] { r, feedback.Rating }.Average());
            rating.Value = r.ToString();
            var author = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
            string authorFirstName = author.Attributes.First(x => x.Name == nameof(Data.Entities.Users.AttributeTypes.FirstName));

            string userFirstName = user.Attributes.First(x => x.Name == nameof(Data.Entities.Users.AttributeTypes.FirstName));
            string userEmail = user.Attributes.First(x => x.Name == nameof(Data.Entities.Users.AttributeTypes.EmailAddress));

            user.Feedback.Add(new Data.Shared.Feedback
            {
                Author = author,
                Rating = feedback.Rating,
                Text = feedback.Text
            });

            await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
            {
                Content = $@"
Hi, {userFirstName}!
You've got a new feedback from {authorFirstName}

{feedback.Text}
...
Rating: {feedback.Rating} star(s).",
                Subject = "New feedback",
                ToName = userFirstName,
                To = userEmail
            });
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }
    }
}
