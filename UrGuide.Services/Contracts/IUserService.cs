using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Users;

namespace UrGuide.Services.Contracts
{
    public interface IUserService
    {
        Task<Result<User>> GetUserAsync(string userId, CancellationToken cancellationToken);
        Task<Result<bool>> SetUserAttributeAsync(string userId, SetUserAttribute attribute, CancellationToken cancellationToken);
        Task<Result<bool>> DeleteUserAccountAsync(string userId, CancellationToken cancellationToken);
        Task<Result<bool>> RegisterUserAsync(CreateUserModel createUser, CancellationToken cancellationToken);
        Task<Result<bool>> RegisterGuideAsync(CreateGuideModel createGuide, CancellationToken cancellationToken);
        Task<Result<User>> LoginAsync(LoginModel login, CancellationToken cancellationToken);
    }
}
