using System.Threading;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Users;

namespace UrGuide.Services.Contracts
{
    public interface IUserService
    {
        Task<Outcome<User>> GetUserAsync(string userId, CancellationToken cancellationToken);
        Task<Outcome<bool>> ExistsAsync(string userId, CancellationToken cancellationToken);
        Task<Outcome<bool>> SetUserAttributeAsync(SetAttribute attribute, CancellationToken cancellationToken);
        Task<Outcome<bool>> DeleteUserAccountAsync(CancellationToken cancellationToken);
        Task<Outcome<bool>> RegisterUserAsync(CreateUserModel createUser, CancellationToken cancellationToken);
        Task<Outcome<bool>> RegisterGuideAsync(CreateGuideModel createGuide, CancellationToken cancellationToken);
        Task<Outcome<User>> LoginAsync(LoginModel login, CancellationToken cancellationToken);
        Task<Outcome<bool>> UpdateGuideAsync(UpdateGuideModel updateGuide, CancellationToken cancellationToken);
        Task<Outcome<bool>> UpdateUserAsync(UpdateUserModel updateUser, CancellationToken cancellationToken);
        Task<Outcome<UserInfo>> GetUserInfo(string userId, CancellationToken cancellationToken);
        Task<Outcome<User>> GetDetailsAsync(CancellationToken cancellationToken);
        Task<Outcome<PagedList<UserInfo>>> GetUsersAsync(SearchParameters searchParameters, CancellationToken cancellationToken);
        Task<Outcome<UserDataExport>> GetUserDataExportAsync(CancellationToken cancellationToken);
    }
}
