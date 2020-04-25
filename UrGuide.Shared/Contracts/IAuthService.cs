using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Users;

namespace UrGuide.Shared.Contracts
{
    public interface IAuthService
    {
        Task<Result<(string userId, string confirmationToken)>> RegisterUserAsync(CreateUserCommand createUser, CancellationToken cancellationToken);
        Task<Result<(string userId, string confirmationToken)>> RegisterGuideAsync(CreateGuideCommand createGuide, CancellationToken cancellationToken);
        Task<Result<string>> LoginAsync(LoginCommand login, CancellationToken cancellationToken);
    }
}
