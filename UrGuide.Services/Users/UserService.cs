using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Model.Results;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Users
{
    public class UserService : IUserService
    {
        public UserService(UrGuideContext context,
            IUserContext userContext,
            IAuthService authService,
            ILogger<UserService> logger,
            IMapper mapper)
        {
            Context = context ?? throw new System.ArgumentNullException(nameof(context));
            UserContext = userContext ?? throw new System.ArgumentNullException(nameof(userContext));
            AuthService = authService ?? throw new System.ArgumentNullException(nameof(authService));
            Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            Mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public UrGuideContext Context { get; }
        public IUserContext UserContext { get; }
        public IAuthService AuthService { get; }
        public ILogger<UserService> Logger { get; }
        public IMapper Mapper { get; }

        public async Task<Result<bool>> DeleteUserAccountAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var user = await Context.Users.FindAsync(userId, cancellationToken);
                Context.Users.Remove(user);
                await Context.SaveChangesAsync(cancellationToken);
                return Result.Of(true);
            }
            catch (System.Exception e)
            {
                Logger.LogError(e, "Failed to delete a user account: UserId: {0}", userId);
                return Result.Of(false).WithErrors("Failed to delete a user account");
            }
        }

        public async Task<Result<User>> GetUserAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await Context.Users.FindAsync(userId, cancellationToken);
            return Result.Of(Mapper.Map<User>(user));
        }

        public async Task<Result<User>> LoginAsync(LoginCommand login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userId = await AuthService.LoginAsync(login, cancellationToken);
            if (userId.HasError)
                return Result.Of<User>(null).Combine(userId);
            return await GetUserAsync(userId.Data, cancellationToken);
        }

        public async Task<Result<bool>> RegisterGuideAsync(CreateGuideCommand createGuide, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Result<string> userId = await AuthService.RegisterGuideAsync(createGuide, cancellationToken);
            if (userId.HasError)
            {
                return Result.Of(false).Combine(userId);
            }
            var user = new Data.Entities.Users.User
            {
                UserId = userId.Data,
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
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }

        public async Task<Result<bool>> RegisterUserAsync(CreateUserCommand createUser, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Result<string> userId = await AuthService.RegisterUserAsync(createUser, cancellationToken);
            if (userId.HasError)
            {
                return Result.Of(false).Combine(userId);
            }
            var user = new Data.Entities.Users.User
            {
                UserId = userId.Data
            };
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.EmailOptIn), Value = Constants.Yes });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.EmailAddress), Value = createUser.Email });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.UserName), Value = createUser.Email });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.NickName), Value = createUser.Email });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Consent), Value = Constants.Yes });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.GuideOptIn), Value = Constants.No });
            user.Attributes.Add(new Data.Entities.Attributes.GenericAttribute { Name = nameof(Data.Entities.Users.AttributeTypes.Subscription), Value = nameof(Subscriptions.None) });

            Context.Users.Add(user);
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }

        public async Task<Result<bool>> SetUserAttributeAsync(string userId, SetUserAttribute attribute, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await Context.Users.FindAsync(userId, cancellationToken);
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
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }
    }
}
