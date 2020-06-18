using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Users;

namespace UrGuide.Shared.Contracts
{
    public interface IAuthService
    {
        Task<Result<(string userId, string confirmationToken)>> RegisterUserAsync(CreateUserModel createUser, CancellationToken cancellationToken);
        Task<Result<(string userId, string confirmationToken)>> RegisterGuideAsync(CreateGuideModel createGuide, CancellationToken cancellationToken);
        Task<Result<string>> LoginAsync(LoginModel login, CancellationToken cancellationToken);
        Task<Result<bool>> ConfirmEmailAsync(EmailConfirmationModel emailConfirmation, CancellationToken cancellationToken);
        Task<Result<bool>> RequestPasswordResetAsync(PasswordResetRequestModel passwordResetRequest, CancellationToken cancellationToken);
        Task<Result<bool>> ResetPasswordAsync(ResetPasswordModel resetPasswordModel, CancellationToken cancellationToken);
        Task<Result<bool>> ChangePasswordAsync(ChangePasswordModel model, CancellationToken cancellationToken);
        Task SignOutAsync();
        Task<Result<bool>> DeleteAccount();
        Task DeleteAccountAsync(string userId);
    }
}
