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
        public AuthService(SignInManager<UrGuideUser> signInManager, 
            IUserContext userContext, 
            IEmailService emailService,
            IWebHelper webHelper)
        {
            SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            EmailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            WebHelper = webHelper ?? throw new ArgumentNullException(nameof(webHelper));
        }

        public SignInManager<UrGuideUser> SignInManager { get; }
        public IUserContext UserContext { get; }
        public IEmailService EmailService { get; }
        public IWebHelper WebHelper { get; }

        public async Task<Result<bool>> ChangePasswordAsync(ChangePasswordModel model, CancellationToken cancellationToken)
        {
            var userManager = SignInManager.UserManager;
            var user = await userManager.FindByEmailAsync(model.Email);
            if (!UserContext.IsAuthenticated|| user?.Id != UserContext.UserId)
            {
                if (user != null)
                    await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
                    {
                        Content = "An attempt was made to change your password.",
                        Subject = "Change password",
                        To = user.Email,
                        ToName = user.FirstName
                    }).ConfigureAwait(false);
                return Result.Of(false).WithErrors("Failed to change a user's password");
            }

            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);
            if (result.Succeeded)
            {
                await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
                {
                    Content = "Your password was changed successfully.",
                    Subject = "Change password",
                    To = user.Email,
                    ToName = user.FirstName
                }).ConfigureAwait(false);
                return Result.Of(true);
            }
            return Result.Of(false).WithErrors("Failed to change a user's password");
        }

        public async Task<Result<bool>> ConfirmEmailAsync(EmailConfirmationModel emailConfirmation, CancellationToken cancellationToken)
        {
            var userManager = SignInManager.UserManager;
            var user = await userManager.FindByEmailAsync(emailConfirmation.Email);
            var result = await userManager.ConfirmEmailAsync(user, emailConfirmation.ConfirmationToken);
            if (result.Succeeded)
                return Result.Of(true);
            return Result.Of(false).WithErrors("Email confirmation failed");
        }

        public async Task<Result<string>> LoginAsync(LoginModel login, CancellationToken cancellationToken)
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

        public async Task<Result<(string userId, string confirmationToken)>> RegisterGuideAsync(CreateGuideModel createGuide, CancellationToken cancellationToken)
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

        public async Task<Result<(string userId, string confirmationToken)>> RegisterUserAsync(CreateUserModel createUser, CancellationToken cancellationToken)
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

        public async Task<Result<bool>> RequestPasswordResetAsync(PasswordResetRequestModel passwordResetRequest, CancellationToken cancellationToken)
        {
            var userManager = SignInManager.UserManager;
            var user = await userManager.FindByEmailAsync(passwordResetRequest.Email).ConfigureAwait(false);
            if (user != null)
            {
                var confirmationToken = await userManager.GeneratePasswordResetTokenAsync(user);

                await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
                {
                    Content = "Please reset your password",
                    Link = WebHelper.ResolveUrl(Shared.MessageTypes.PasswordReset, new ResetPasswordModel
                    {
                        ConfirmationToken = confirmationToken,
                        Email = passwordResetRequest.Email
                    }),
                    LinkText = "Reset your password",
                    Subject = "Password reset",
                    To = passwordResetRequest.Email,
                    ToName = user.FirstName
                }).ConfigureAwait(false);

                return Result.Of(true);
            }
            return Result.Of(false).WithErrors("Failed to generate a password reset token");
        }

        public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordModel resetPasswordModel, CancellationToken cancellationToken)
        {
            var userManager = SignInManager.UserManager;

            var user = await userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user != null)
            {
                var confirmationResult = await userManager.ResetPasswordAsync(user, resetPasswordModel.ConfirmationToken, resetPasswordModel.Password);
                if (confirmationResult.Succeeded)
                {
                    await EmailService.SendAsync(new Model.Messages.SendDirectMessageCommand
                    {
                        Subject = "Password reset",
                        Content = "Your password was successfully reset.",
                        To = user.Email,
                        ToName = user.FirstName
                    }).ConfigureAwait(false);
                    return Result.Of(true);
                }
            }
            return Result.Of(false).WithErrors("Failed to reset your password");
        }
    }
}
