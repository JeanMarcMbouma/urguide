using System.Threading;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Model.Results;
using UrGuide.Model.Users;

namespace UrGuide.Shared.Contracts
{
    public interface IAuthService
    {
        Task<Outcome<(string userId, string confirmationToken)>> RegisterUserAsync(CreateUserModel createUser, CancellationToken cancellationToken);
        Task<Outcome<(string userId, string confirmationToken)>> RegisterGuideAsync(CreateGuideModel createGuide, CancellationToken cancellationToken);
        Task<Outcome<string>> LoginAsync(LoginModel login, CancellationToken cancellationToken);
        Task<Outcome<bool>> ConfirmEmailAsync(EmailConfirmationModel emailConfirmation, CancellationToken cancellationToken);
        Task<Outcome<bool>> RequestPasswordResetAsync(PasswordResetRequestModel passwordResetRequest, CancellationToken cancellationToken);
        Task<Outcome<bool>> ResetPasswordAsync(ResetPasswordModel resetPasswordModel, CancellationToken cancellationToken);
        Task<Outcome<bool>> ChangePasswordAsync(ChangePasswordModel model, CancellationToken cancellationToken);
        Task SignOutAsync();
        Task<Outcome<bool>> DeleteAccount();
        ValueTask DeleteAccountAsync(string userId);
    }
}
