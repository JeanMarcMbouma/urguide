using Duende.IdentityModel;
using Microsoft.AspNetCore.SignalR;

namespace UrGuide.WebApp.Services
{
    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(JwtClaimTypes.Subject)?.Value ?? string.Empty;
        }
    }
}
