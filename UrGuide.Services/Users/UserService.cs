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
            IWebHelper webHelper)
        {
            Context = context ?? throw new System.ArgumentNullException(nameof(context));
            UserContext = userContext ?? throw new System.ArgumentNullException(nameof(userContext));
            AuthService = authService ?? throw new System.ArgumentNullException(nameof(authService));
            Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            Mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
            EmailService = emailService ?? throw new System.ArgumentNullException(nameof(emailService));
            WebHelper = webHelper ?? throw new System.ArgumentNullException(nameof(webHelper));
        }

        public UrGuideContext Context { get; }
        public IUserContext UserContext { get; }
        public IAuthService AuthService { get; }
        public ILogger<UserService> Logger { get; }
        public IMapper Mapper { get; }
        public IEmailService EmailService { get; }
        public IWebHelper WebHelper { get; }

        public async Task<Result<bool>> DeleteUserAccountAsync(CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var user = await Context.Users.FindAsync(new { UserContext.UserId }, cancellationToken);
                Context.Users.Remove(user);
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
            var user = await Context.Users.FindAsync(userId, cancellationToken);
            return Result.Of(Mapper.Map<User>(user));
        }

        public async Task<Result<User>> LoginAsync(LoginModel login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userId = await AuthService.LoginAsync(login, cancellationToken);
            if (userId.HasError)
                return Result.Of<User>(null).Combine(userId);
            var user = await GetUserAsync(userId.Data, cancellationToken);
            return user;
        }

        public async Task<Result<bool>> RegisterGuideAsync(CreateGuideModel createGuide, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Result<(string userId, string confirmationToken)> userId = await AuthService.RegisterGuideAsync(createGuide, cancellationToken);
            if (userId.HasError)
            {
                return Result.Of(false).Combine(userId);
            }
            var user = new Data.Entities.Users.User
            {
                UserId = userId.Data.userId,
                ProfileImage = new Data.Entities.Users.Image
                {
                    ImageBase64 = createGuide.ProfileImage
                }
            };
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.EmailOptIn), Value = Constants.Yes });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.EmailAddress), Value = createGuide.Email });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.UserName), Value = createGuide.Email });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.NickName), Value = createGuide.Email });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.FirstName), Value = createGuide.FirstName });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.LastName), Value = createGuide.LastName });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.NickName), Value = createGuide.Email });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Consent), Value = Constants.Yes });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.GuideOptIn), Value = Constants.Yes });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Phone), Value = createGuide.Phone });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Country), Value = createGuide.Country });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.City), Value = createGuide.City });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Address), Value = createGuide.Address });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Gender), Value = createGuide.Gender });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.BirthDay), Value = createGuide.BirthDay });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Subscription), Value = nameof(Subscriptions.Premium) });

            Context.Users.Add(user);
            await EmailService.SendAsync(new SendDirectMessageCommand
            {
                To = createGuide.Email,
                ToName = createGuide.FirstName,
                Content = "Please confirm your account",
                Subject = "Email Confirmation",
                Link = WebHelper.ResolveUrl(MessageTypes.Confirmation, new { userId.Data.confirmationToken, createGuide.Email })
            });
            return Result.Of(true);
        }

        public async Task<Result<bool>> RegisterUserAsync(CreateUserModel createUser, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Result<(string userId, string confirmationToken)> userId = await AuthService.RegisterUserAsync(createUser, cancellationToken);
            if (userId.HasError)
            {
                return Result.Of(false).Combine(userId);
            }
            var user = new Data.Entities.Users.User
            {
                UserId = userId.Data.userId
            };
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.EmailOptIn), Value = Constants.Yes });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.EmailAddress), Value = createUser.Email });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.UserName), Value = createUser.Email });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.NickName), Value = createUser.Email });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Consent), Value = Constants.Yes });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.GuideOptIn), Value = Constants.No });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Subscription), Value = nameof(Subscriptions.None) });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.FirstName), Value = createUser.FirstName });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.LastName), Value = createUser.LastName });

            Context.Users.Add(user);
            await EmailService.SendAsync(new SendDirectMessageCommand
            {
                To = createUser.Email,
                ToName = createUser.FirstName,
                Content = "Please confirm your account",
                Subject = "Email Confirmation",
                LinkText = "Activate your account",
                Link = WebHelper.ResolveUrl(MessageTypes.Confirmation, new { userId.Data.confirmationToken, createUser.Email })
            });
            return Result.Of(true);
        }

        public async Task<Result<bool>> SetUserAttributeAsync(SetAttribute attribute, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();
            var user = await Context.Users.FindAsync(new { UserContext.UserId }, cancellationToken);
            var attributes = user.Attributes;
            var attr = attributes.FirstOrDefault(a => a.Name == attribute.Name);
            if (attr == null)
            {
                user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute
                {
                    Name = attribute.Name,
                    Value = attribute.Value
                });
            }
            else
            {
                attr.Value = attribute.Value;
            }
            return Result.Of(true);
        }
    }
}
