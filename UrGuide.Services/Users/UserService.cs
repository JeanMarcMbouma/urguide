using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Model;
using UrGuide.Model.Messages;
using UrGuide.Model.Results;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.Shared;
using UrGuide.Shared.Contracts;
using UrGuide.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using MediatR;
using UrGuide.Services.Auditing.Command;
using System.ComponentModel.DataAnnotations;
using UrGuide.Core.Attributes;
using UrGuide.Core;
using System;
using System.Collections.Generic;
using UrGuide.Model.Shared;

namespace UrGuide.Services.Users
{
    public class UserService : IUserService
    {
        public UserService(UrGuideContext context,
            IUserContext userContext,
            IAuthService authService,
            ILogger<UserService> logger,
            IMapper mapper,
            IEmailService emailService,
            IWebHelper webHelper,
            IIPStackService iPStackService,
            IImageService imageService,
            IMediator mediator)
        {
            Context = context ?? throw new System.ArgumentNullException(nameof(context));
            UserContext = userContext ?? throw new System.ArgumentNullException(nameof(userContext));
            AuthService = authService ?? throw new System.ArgumentNullException(nameof(authService));
            Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            Mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
            EmailService = emailService ?? throw new System.ArgumentNullException(nameof(emailService));
            WebHelper = webHelper ?? throw new System.ArgumentNullException(nameof(webHelper));
            IPStackService = iPStackService ?? throw new System.ArgumentNullException(nameof(iPStackService));
            ImageService = imageService ?? throw new System.ArgumentNullException(nameof(imageService));
            Mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public UrGuideContext Context { get; }
        public IUserContext UserContext { get; }
        public IAuthService AuthService { get; }
        public ILogger<UserService> Logger { get; }
        public IMapper Mapper { get; }
        public IEmailService EmailService { get; }
        public IWebHelper WebHelper { get; }
        public IIPStackService IPStackService { get; }
        public IImageService ImageService { get; }
        public IMediator Mediator { get; }

        public async Task<Result<bool>> DeleteUserAccountAsync(CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var r = await AuthService.DeleteAccount();
                if (r.HasError)
                    return r;
                var user = await Context.Users.FindAsync(new []{ UserContext.UserId }, cancellationToken);
                Context.Users.Remove(user);
                await Context.SaveChangesAsync(cancellationToken);
                await Mediator.Send(new UserDeleteAccountCommand(UserContext.UserId));
                return Result.Of(true);
            }
            catch (System.Exception e)
            {
                Logger.LogError(e, "Failed to delete a user account: UserId: {0}", UserContext.UserId);
                return Result.Of(false).WithErrors("Failed to delete a user account");
            }
        }

        public async Task<Result<User>> GetUserAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await Context.Users.FindAsync(new[] { userId }, cancellationToken);
            return Result.Of(Mapper.Map<User>(user));
        }

        public async Task<Result<User>> LoginAsync(LoginModel login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userId = await AuthService.LoginAsync(login, cancellationToken);
            if (userId.HasError)
                return Result.Of<User>().Combine(userId);
            var user = await Context.Users.FindAsync(new[] { userId.Data }, cancellationToken);
            if(user == null)
                return Result.Of<User>().WithErrors("Invalid login attempt.");
            user.LastActivityDate = System.DateTime.UtcNow;
            await Mediator.Send(new UserLoggedInCommand(user.Id));
            return Result.Of(Mapper.Map<User>(user));
        }

        public async Task<Result<bool>> RegisterGuideAsync(CreateGuideModel createGuide, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Result<(string userId, string confirmationToken)> result = await AuthService.RegisterGuideAsync(createGuide, cancellationToken);
            if (result.HasError)
            {
                return Result.Of(false).Combine(result);
            }
            try
            {

                var imageUrl = ImageService.SaveAvatar(result.Data.userId, new Model.Shared.ImageFileModel
                {
                    ImageBase64 = createGuide.ProfileImage
                });

                var user = new Data.Entities.Users.User
                {
                    Id = result.Data.userId,
                    ProfileImage = new Data.Entities.Users.Image
                    {
                        ImageUrl = imageUrl
                    }
                };
                user.FirstName = createGuide.FirstName;
                user.LastName = createGuide.LastName;
                user.Email = createGuide.Email;
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.EmailOptIn), Value = Constants.Yes });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.UserName), Value = createGuide.Email });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.NickName), Value = createGuide.Email });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Consent), Value = Constants.Yes });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.GuideOptIn), Value = Constants.Yes });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Phone), Value = createGuide.Phone });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Country), Value = createGuide.Country });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.City), Value = createGuide.City });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Address), Value = createGuide.Address });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Gender), Value = createGuide.Gender });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.BirthDay), Value = createGuide.BirthDay });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Description), Value = createGuide.Description });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Subscription), Value = nameof(Subscriptions.Premium) });

                await user.SetLocationAsync(UserContext, IPStackService);

                Context.Users.Add(user);
                await EmailService.SendAsync(new SendDirectMessageCommand
                {
                    To = createGuide.Email,
                    ToName = createGuide.FirstName,
                    Content = "Please confirm your account",
                    Subject = "Email Confirmation",
                    LinkText = "Activate your account",
                    Link = WebHelper.ResolveUrl(MessageTypes.Confirmation, new { result.Data.confirmationToken, createGuide.Email })
                });
                return Result.Of(true);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (System.Exception e)
            {
                Logger.LogError(e, "Error occured during registration");
                await AuthService.DeleteAccountAsync(result.Data.userId);
                return Result.Of(false).WithErrors("An error has occured during user's registration.", e.Message);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public async Task<Result<bool>> RegisterUserAsync(CreateUserModel createUser, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Result<(string userId, string confirmationToken)> result = await AuthService.RegisterUserAsync(createUser, cancellationToken);
            if (result.HasError)
            {
                return Result.Of(false).Combine(result);
            }
            try
            {
                var imageUrl = ImageService.SaveAvatar(result.Data.userId);

                var user = new Data.Entities.Users.User
                {
                    Id = result.Data.userId,
                    ProfileImage = new Data.Entities.Users.Image
                    {
                        ImageUrl = imageUrl
                    }
                };

                user.FirstName = createUser.FirstName;
                user.LastName = createUser.LastName;
                user.Email = createUser.Email;

                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.EmailOptIn), Value = Constants.Yes });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.UserName), Value = createUser.Email });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.NickName), Value = createUser.Email });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Consent), Value = Constants.Yes });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.GuideOptIn), Value = Constants.No });
                user.Attributes.Add(new GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Subscription), Value = nameof(Subscriptions.None) });

                await user.SetLocationAsync(UserContext, IPStackService);

                Context.Users.Add(user);

                await EmailService.SendAsync(new SendDirectMessageCommand
                {
                    To = createUser.Email,
                    ToName = createUser.FirstName,
                    Content = "Please confirm your account",
                    Subject = "Email Confirmation",
                    LinkText = "Activate your account",
                    Link = WebHelper.ResolveUrl(MessageTypes.Confirmation, new { result.Data.confirmationToken, createUser.Email })
                });
                return Result.Of(true);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (System.Exception e)
            {
                Logger.LogError(e, "Error occured during registration");
                await AuthService.DeleteAccountAsync(result.Data.userId);
                return Result.Of(false).WithErrors("An error has occured during registration.", e.Message);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public async Task<Result<bool>> SetUserAttributeAsync(SetAttribute attribute, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();
            var user = await Context.Users.FindAsync(new { UserContext.UserId }, cancellationToken);
            SetAttributesInternal(new []{ attribute }, user);
            return Result.Of(true);
        }

        private static void SetAttributesInternal(SetAttribute[] attributes, Data.Entities.Users.User user)
        {
            var attrs = user.Attributes;
            foreach (var attribute in attributes)
            {
                var attr = attrs.FirstOrDefault(a => a.Name.Equals(attribute.Name, System.StringComparison.OrdinalIgnoreCase));
                if (attr == null)
                {
                    user.Attributes.Add(new GenericAttribute
                    {
                        Name = attribute.Name,
                        Value = attribute.Value
                    });
                }
                else
                {
                    attr.Value = attribute.Value;
                }
            }
        }

        public async Task<Result<bool>> UpdateGuideAsync(UpdateGuideModel updateGuide, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var user = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
            if (updateGuide.ProfileImage != null)
            {
                var imageUrl = ImageService.SaveAvatar(UserContext.UserId, new Model.Shared.ImageFileModel
                {
                    ImageBase64 = updateGuide.ProfileImage
                });

                user.ProfileImage.ImageUrl = imageUrl;
            }
            user.FirstName = updateGuide.FirstName;
            user.LastName = updateGuide.LastName;
            var attributes = new[]{ 
                    new SetAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Phone), Value = updateGuide.Phone },
                    new SetAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Country), Value = updateGuide.Country },
                    new SetAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.City), Value = updateGuide.City },
                    new SetAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Address), Value = updateGuide.Address },
                    new SetAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Gender), Value = updateGuide.Gender } ,
                    new SetAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.BirthDay), Value = updateGuide.BirthDay } ,
                    new SetAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Description), Value = updateGuide.Description }
                };

            SetAttributesInternal(attributes, user);

            return Result.Of(true);
        }

        public async Task<Result<User>> GetDetailsAsync(CancellationToken cancellationToken)
        { 

            if (!UserContext.IsAuthenticated)
                return Result.Of<User>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();
            var user = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
            if (user == null)
                return Result.Of<User>().WithErrors("User not found.");

            return Result.Of(Mapper.Map<User>(user));

        }

        public async Task<Result<bool>> ExistsAsync(string userId, CancellationToken cancellationToken)
        {
            var result = await Context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
            return Result.Of(result);
        }

        public async Task<Result<UserInfo>> GetUserInfo(string userId, CancellationToken cancellationToken)
        {
            var result = await Context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (result == null)
                return Result.Of<UserInfo>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            return Result.Of(Mapper.Map<UserInfo>(result));
        }

        public async Task<Result<bool>> UpdateUserAsync(UpdateUserModel updateUser, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var user = await Context.Users.FindAsync(new[] { UserContext.UserId }, cancellationToken);
            if (updateUser.ProfileImage != null)
            {
                var imageUrl = ImageService.SaveAvatar(UserContext.UserId, new Model.Shared.ImageFileModel
                {
                    ImageBase64 = updateUser.ProfileImage
                });

                user.ProfileImage.ImageUrl = imageUrl;
            }
            user.LastName = updateUser.LastName;
            user.FirstName = updateUser.FirstName;

            return Result.Of(true);
        }

        public async Task<Result<PagedList<UserInfo>>> GetUsersAsync(SearchParameters searchParameters, CancellationToken cancellationToken)
        {
            var geo = searchParameters.Nearby ? await IPStackService.GetLocationAsync(UserContext) : null;
            var users = await PagedList.Of(Context.Users
                .Where(x => geo == null || x.Location.Distance(geo) <= Constants.Distance)
                .Where(x => EF.Functions.Like(x.FirstName, $"%{searchParameters.Term}%")
                || EF.Functions.Like(x.LastName, $"%{searchParameters.Term}%")), 
                searchParameters.PageNumber
                , u => Mapper.Map<UserInfo>(u), cancellationToken);
            return Result.Of(users);
        }

        public async Task<Result<UserDataExport>> GetUserDataExportAsync(CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<UserDataExport>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var userId = UserContext.UserId;
                
                // Get user with all related data
                var user = await Context.Users
                    .Include(u => u.Attributes)
                    .Include(u => u.Feedback).ThenInclude(f => f.Author)
                    .Include(u => u.Notifications)
                    .Include(u => u.ProfileImage)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (user == null)
                    return Result.Of<UserDataExport>().WithErrors("User not found.");

                // Get user's posts
                var userPosts = await Context.Posts
                    .Where(p => p.User.Id == userId)
                    .Include(p => p.Feedback).ThenInclude(f => f.Author)
                    .Include(p => p.BidHistories)
                    .Include(p => p.Itineraries)
                    .Include(p => p.Reservations)
                    .Include(p => p.UserReactions)
                    .ToListAsync(cancellationToken);

                // Get user's galleries
                var userGalleries = await Context.ImageCatalogs
                    .Where(g => g.User.Id == userId)
                    .Include(g => g.Images)
                    .Include(g => g.Attributes)
                    .ToListAsync(cancellationToken);

                // Get user's tour requests
                var userTourRequests = await Context.TourRequests
                    .Where(tr => tr.RequesterId == userId)
                    .Include(tr => tr.Region)
                    .ToListAsync(cancellationToken);

                // Get feedback given by user (on posts)
                var givenFeedback = await Context.Posts
                    .SelectMany(p => p.Feedback)
                    .Where(f => f.Author.Id == userId)
                    .Include(f => f.Author)
                    .ToListAsync(cancellationToken);

                // Get user's audit events/activity
                var userActivity = await Context.AuditEvents
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.Created)
                    .ToListAsync(cancellationToken);

                // Map user attributes to dictionary
                var userAttributes = user.Attributes.ToDictionary(a => a.Name, a => a.Value);

                // Create the export data
                var exportData = new UserDataExport
                {
                    ExportDate = DateTime.UtcNow,
                    Profile = Mapper.Map<UserInfo>(user),
                    Attributes = userAttributes,
                    GivenFeedback = givenFeedback.Select(f => Mapper.Map<AuthoredFeedback>(f)).ToList(),
                    ReceivedFeedback = user.Feedback.Select(f => Mapper.Map<AuthoredFeedback>(f)).ToList(),
                    Galleries = userGalleries.Select(g => new
                    {
                        Id = g.Id,
                        Created = g.Created,
                        LastUpdated = g.LastUpdated,
                        Images = g.Images.Select(i => new { i.ImageUrl }).ToList(),
                        Attributes = g.Attributes.ToDictionary(a => a.Name, a => a.Value),
                        Location = g.Location != null ? new { Lat = g.Location.Y, Lng = g.Location.X } : null
                    }).Cast<object>().ToList(),
                    Posts = userPosts.Select(p => new
                    {
                        Id = p.Id,
                        Text = p.Text,
                        Description = p.Description,
                        DateOfPublication = p.DateOfPublication,
                        LastUpdated = p.LastUpdated,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        Cost = p.Cost,
                        Tags = p.Tags,
                        BidEnabled = p.BidEnabled,
                        Rating = p.Rating,
                        Reviews = p.Reviews,
                        Likes = p.Likes,
                        Dislikes = p.Dislikes,
                        Feedback = p.Feedback.Select(f => new
                        {
                            Text = f.Text,
                            Rating = f.Rating,
                            Created = f.Created,
                            AuthorName = f.Author?.FirstName + " " + f.Author?.LastName
                        }).ToList(),
                        BidHistory = p.BidHistories.Select(b => new
                        {
                            Value = b.Value,
                            Created = b.Created
                        }).ToList()
                    }).Cast<object>().ToList(),
                    Notifications = user.Notifications.Select(n => new
                    {
                        Id = n.Id,
                        Content = n.Content,
                        ReferenceLink = n.ReferenceLink,
                        Created = n.Created,
                        IsRead = n.Read,
                        IsSystem = n.IsSystem
                    }).Cast<object>().ToList(),
                    ActivityHistory = userActivity.Select(a => new
                    {
                        EventCode = a.EventCode.ToString(),
                        TimeStamp = a.Created,
                        UserId = a.UserId,
                        ReferenceId = a.ReferenceId
                    }).Cast<object>().ToList(),
                    TourRequests = userTourRequests.Select(tr => new
                    {
                        Id = tr.TourRequestId,
                        Title = tr.Title,
                        Description = tr.Description,
                        MaxBudget = tr.MaxBudget,
                        PreferredDate = tr.PreferredDate,
                        MaxParticipants = tr.MaxParticipants,
                        Tags = tr.Tags,
                        Status = tr.Status.ToString(),
                        Created = tr.CreatedAt,
                        Updated = tr.UpdatedAt,
                        Region = tr.Region?.Name
                    }).Cast<object>().ToList(),
                    Account = new AccountMetadata
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        LastActivityDate = user.LastActivityDate,
                        CreatedDate = userActivity.OrderBy(a => a.Created).FirstOrDefault()?.Created ?? DateTime.MinValue,
                        IsGuide = userAttributes.ContainsKey(nameof(Data.Entities.Users.AttributeTypes.GuideOptIn)) && 
                                 userAttributes[nameof(Data.Entities.Users.AttributeTypes.GuideOptIn)] == Constants.Yes,
                        IsPremium = userAttributes.ContainsKey(nameof(Data.Entities.Users.AttributeTypes.Subscription)) && 
                                   userAttributes[nameof(Data.Entities.Users.AttributeTypes.Subscription)] == nameof(Subscriptions.Premium)
                    }
                };

                return Result.Of(exportData);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to export user data: UserId: {0}", UserContext.UserId);
                return Result.Of<UserDataExport>().WithErrors("Failed to export user data", e.Message);
            }
        }
    }
}
