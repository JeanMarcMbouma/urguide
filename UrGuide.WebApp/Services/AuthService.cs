using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Users;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Entities;

namespace UrGuide.WebApp.Services
{
    public class AuthService : IAuthService
    {
        public AuthService(SignInManager<UrGuideUser> signInManager, IUserContext userContext)
        {
            SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public SignInManager<UrGuideUser> SignInManager { get; }
        public IUserContext UserContext { get; }

        public async Task<Result<string>> LoginAsync(LoginCommand login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await SignInManager.PasswordSignInAsync(login.UserName, login.Password, login.Persist, true);
            if (!result.Succeeded)
            {
                return Result.Of<string>().WithErrors("Invalid login attempt.");
            }
            if (result.IsLockedOut)
            {
                return Result.Of<string>().WithErrors("Your account has been locked out.");
            }
            var user = await SignInManager.UserManager.FindByNameAsync(login.UserName);
            
            return Result.Of(user.Id);
        }

        public async Task<Result<(string userId, string confirmationToken)>> RegisterGuideAsync(CreateGuideCommand createGuide, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userManager = SignInManager.UserManager;
            var user = await userManager.FindByNameAsync(createGuide.Email);
            if (user != null)
            {
                return Result.Of<(string userId, string confirmationToken)>().WithErrors("Account already exist");
            }
            user = new UrGuideUser
            {
                Email = createGuide.Email,
                FirstName = createGuide.FirstName,
                LastName = createGuide.LastName,
                IsGuide = true,
                PhoneNumber = createGuide.Phone,
                UserName = createGuide.Email
            };

            var result = await userManager.CreateAsync(user, createGuide.Password);
            if (!result.Succeeded)
            {
                var r = Result.Of<(string userId, string confirmationToken)>().WithErrors("User's registration failed.");
                result.Errors.ToList().ForEach(e =>
                {
                    r.WithErrors(e.Description);
                });
                return r;
            }

            var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            return Result.Of((user.Id, emailConfirmationToken));
        }

        public async Task<Result<(string userId, string confirmationToken)>> RegisterUserAsync(CreateUserCommand createUser, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userManager = SignInManager.UserManager;
            var user = await userManager.FindByNameAsync(createUser.Email);
            if (user != null)
            {
                return Result.Of<(string userId, string confirmationToken)>().WithErrors("Account already exist");
            }
            user = new UrGuideUser
            {
                Email = createUser.Email,
                FirstName = createUser.FirstName,
                LastName = createUser.LastName,
                IsGuide = false,
                UserName = createUser.Email
            };
            var result = await userManager.CreateAsync(user, createUser.Password);
            if (!result.Succeeded)
            {
                var r = Result.Of<(string userId, string confirmationToken)>().WithErrors("User's registration failed.");
                result.Errors.ToList().ForEach(e =>
                {
                    r.WithErrors(e.Description);
                });
                return r;
            }

            var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            return Result.Of((user.Id, emailConfirmationToken));
        }
    }
}
