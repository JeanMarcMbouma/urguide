using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Hubs;

namespace UrGuide.WebApp.Services
{
    public class InstantMessagingService : IInstantMessagingService
    {
        public InstantMessagingService(IHubContext<NotificationHub> hubContext)
        {
            HubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public IHubContext<NotificationHub> HubContext { get; }

        public Task Send<T>(string userId, T message)
        {
            return HubContext.Clients.All./*User(userId).*/SendAsync("notify", message, userId);
        }

        public Task Send<T, TUserInfo>(string userId, T message, TUserInfo userInfo)
        {
            return HubContext.Clients.User(userId).SendAsync("notify", message, userInfo);
        }
    }
}
