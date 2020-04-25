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
        Task<Result<bool>> RegisterUserAsync(CreateUserCommand createUser, CancellationToken cancellationToken);
        Task<Result<bool>> RegisterGuideAsync(CreateGuideCommand createGuide, CancellationToken cancellationToken);
        Task<Result<User>> LoginAsync(LoginCommand login, CancellationToken cancellationToken);
    }
}
