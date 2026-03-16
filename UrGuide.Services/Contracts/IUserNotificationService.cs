using System.Threading;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Users;

namespace UrGuide.Services.Contracts
{
    public interface IUserNotificationService
    {
        Task NotifyAsync(CreateNotification createNotification);
        Task SystemNotifyAsync(string userId, string content, string? referenceLink);
        Task<Outcome<bool>> MarkAsReadAsync(string notificationId, CancellationToken cancellationToken);
        Task<Outcome<Notification>> GetNotificationAsync(string notificationId, CancellationToken cancellationToken);
        Task<Outcome<PagedList<Notification>>> GetUnreadAsync(PaginationParameters pagination, CancellationToken cancellationToken);
        Task<Outcome<PagedList<Notification>>> GetAllAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    }
}
