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
            IImageService imageService)
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
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Description), Value = createGuide.Description });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Subscription), Value = nameof(Subscriptions.Premium) });

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

        public async Task<Result<bool>> RegisterUserAsync(CreateUserModel createUser, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Result<(string userId, string confirmationToken)> result = await AuthService.RegisterUserAsync(createUser, cancellationToken);
            if (result.HasError)
            {
                return Result.Of(false).Combine(result);
            }
            var imageUrl = ImageService.SaveAvatar(result.Data.userId);

            var user = new Data.Entities.Users.User
            {
                Id = result.Data.userId,
                ProfileImage = new Data.Entities.Users.Image
                {
                    ImageUrl = imageUrl
                }
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

            var attributes = new[]{ 
                    new SetAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.FirstName), Value = updateGuide.FirstName },
                    new SetAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.LastName), Value = updateGuide.LastName },
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
    }
}
