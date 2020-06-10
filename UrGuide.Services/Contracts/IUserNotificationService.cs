using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Users;

namespace UrGuide.Services.Contracts
{
    public interface IUserNotificationService
    {
        Task NotifyAsync(CreateNotification createNotification);
        Task SystemNotifyAsync(string userId, string content, string referenceLink);
        Task<Result<PagedList<Notification>>> GetUnreadAsync(PaginationParameters pagination, CancellationToken cancellationToken);
        Task<Result<PagedList<Notification>>> GetAllAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    }
}
