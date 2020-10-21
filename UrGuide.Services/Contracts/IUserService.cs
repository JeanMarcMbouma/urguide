using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Users;

namespace UrGuide.Services.Contracts
{
    public interface IUserService
    {
        Task<Result<User>> GetUserAsync(string userId, CancellationToken cancellationToken);
        Task<Result<bool>> ExistsAsync(string userId, CancellationToken cancellationToken);
        Task<Result<bool>> SetUserAttributeAsync(SetAttribute attribute, CancellationToken cancellationToken);
        Task<Result<bool>> DeleteUserAccountAsync(CancellationToken cancellationToken);
        Task<Result<bool>> RegisterUserAsync(CreateUserModel createUser, CancellationToken cancellationToken);
        Task<Result<bool>> RegisterGuideAsync(CreateGuideModel createGuide, CancellationToken cancellationToken);
        Task<Result<User>> LoginAsync(LoginModel login, CancellationToken cancellationToken);
        Task<Result<bool>> UpdateGuideAsync(UpdateGuideModel updateGuide, CancellationToken cancellationToken);
        Task<Result<bool>> UpdateUserAsync(UpdateUserModel updateUser, CancellationToken cancellationToken);
        Task<Result<UserInfo>> GetUserInfo(string userId, CancellationToken cancellationToken);
        Task<Result<User>> GetDetailsAsync(CancellationToken cancellationToken);
        Task<Result<PagedList<UserInfo>>> GetUsersAsync(SearchParameters searchParameters, CancellationToken cancellationToken);
    }
}
