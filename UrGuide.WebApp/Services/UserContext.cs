using Microsoft.AspNetCore.Http;
using System;
using UrGuide.Shared.Contracts;
using Microsoft.AspNetCore.Identity;
using UrGuide.WebApp.Entities;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Net;

namespace UrGuide.WebApp.Services
{
    public class UserContext : IUserContext
    {
        public UserContext(IHttpContextAccessor httpContextAccessor, 
            SignInManager<UrGuideUser> signInManager)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        public IHttpContextAccessor HttpContextAccessor { get; }
        public SignInManager<UrGuideUser> SignInManager { get; }


        public string UserId => SignInManager.UserManager.GetUserId(HttpContextAccessor.HttpContext.User);
        public string UserName => HttpContextAccessor.HttpContext.User.Identity.Name;
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
