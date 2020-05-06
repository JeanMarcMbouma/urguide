using Microsoft.AspNetCore.Http;
using System;
using UrGuide.Shared.Contracts;
using Microsoft.AspNetCore.Identity;
using UrGuide.WebApp.Entities;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Net;
using System.Security.Claims;

namespace UrGuide.WebApp.Services
{
    public class UserContext : IUserContext
    {
        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public IHttpContextAccessor HttpContextAccessor { get; }

        public string UserId => HttpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        public Task<string> Id_Token => HttpContextAccessor.HttpContext.GetTokenAsync("id_token");
        public Task<string> Access_Token => HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
        public bool IsAuthenticated => HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
#if DEBUG
        public IPAddress IPAddress => IPAddress.Parse("176.67.20.135");
#else
        public IPAddress IPAddress => HttpContextAccessor.HttpContext.Connection.RemoteIpAddress;
#endif
    }
}
