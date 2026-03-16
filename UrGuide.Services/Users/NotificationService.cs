using FluentValidation;
using Microsoft.EntityFrameworkCore;
using BbQ.Outcome;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Data;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Users
{
    class NotificationService : IUserNotificationService
    {
        public NotificationService(UrGuideContext context,
                                   IValidator<CreateNotification> validator,
                                   ILogger<NotificationService> logger,
                                   IUserContext userContext,
                                   IInstantMessagingService instantMessaging)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            InstantMessaging = instantMessaging ?? throw new ArgumentNullException(nameof(instantMessaging));
        }

        public UrGuideContext Context { get; }
        public IValidator<CreateNotification> Validator { get; }
        public ILogger<NotificationService> Logger { get; }
        public IUserContext UserContext { get; }
        public IInstantMessagingService InstantMessaging { get; }

        public async Task<Outcome<PagedList<Model.Users.Notification>>> GetAllAsync(PaginationParameters pagination, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<PagedList<Model.Users.Notification>>().WithErrors(ErrorMessages.NotAuthenticated);
            var user = await Context.Users.FirstAsync(x => x.Id == UserContext.UserId, cancellationToken);
            var items = PagedList.Of(user.Notifications.OrderByDescending(x => x.Created), pagination.PageNumber, n => UserMapper.ToNotification(n));
            return Result.Of(items);
        }

        public async Task<Outcome<Model.Users.Notification>> GetNotificationAsync(string notificationId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<Model.Users.Notification>().WithErrors(ErrorMessages.NotAuthenticated);
            var user = await Context.Users.FirstAsync(x => x.Id == UserContext.UserId, cancellationToken);
            var notification = user?.Notifications.FirstOrDefault(x => x.Id == notificationId);
            if (notification == null)
                return Result.Of<Model.Users.Notification>().WithErrors(ErrorMessages.NotFoundEntityForKey);
            return Result.Of(UserMapper.ToNotification(notification));
        }

        public async Task<Outcome<PagedList<Model.Users.Notification>>> GetUnreadAsync(PaginationParameters pagination, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<PagedList<Model.Users.Notification>>().WithErrors(ErrorMessages.NotAuthenticated);
            var user = await Context.Users.FirstAsync(x => x.Id == UserContext.UserId, cancellationToken);
            var items = PagedList.Of(user.Notifications.Where(m => !m.Read).OrderByDescending(x => x.Created), pagination.PageNumber, n => UserMapper.ToNotification(n));
            return Result.Of(items);
        }

        public async Task<Outcome<bool>> MarkAsReadAsync(string notificationId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == UserContext.UserId, cancellationToken);
            var notification = user?.Notifications.SingleOrDefault(n => n.Id == notificationId);
            if (notification == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);
            notification.Read = true;
            return Result.Of(true);
        }

        public async Task NotifyAsync(CreateNotification createNotification)
        {
            var result = Validator.Validate(createNotification);
            if (result.IsValid)
            {
                var user = await Context.Users.FindAsync(new[] { createNotification.UserId });
                if (user == null)
                {
                    Logger.LogWarning("Notification failed: User with id {UserId} not found", createNotification.UserId);
                    return;
                }
                var sender = await Context.Users.FindAsync(new[] { createNotification.AuthorId });
                var notification = new Data.Entities.Users.Notification
                {
                    Content = createNotification.Content,
                    ReferenceLink = createNotification.ReferenceLink,
                    IsSystem = createNotification.IsSystem,
                    Created = DateTime.UtcNow,
                    Sender = sender,
                    Read = false
                };
                user.Notifications.Add(notification);
                await Context.SaveChangesAsync();
                _ = InstantMessaging.Send(user.Id, UserMapper.ToNotification(notification)).ConfigureAwait(false);
                return;
            }
            Logger.LogWarning("Notification failed: {Errors}", string.Join(Environment.NewLine, result.Errors.Select(x => x.ErrorMessage)));
        }

        public Task SystemNotifyAsync(string userId, string content, string? referenceLink)
        {
            return NotifyAsync(new CreateNotification
            {
                AuthorId = Constants.SystemUserId,
                Content = content,
                ReferenceLink = referenceLink,
                IsSystem = true,
                UserId = userId
            });
        }
    }
}
