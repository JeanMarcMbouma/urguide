using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using UrGuide.Model.Users;
using UrGuide.Shared;
using UrGuide.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace UrGuide.WebApp.Services
{
    public class UserContext : IUserContext
    {
        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public IHttpContextAccessor HttpContextAccessor { get; }

        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string Id_Token { get; private set; }

        public string ProfileImage { get; private set; }

        public bool IsAuthenticated => HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

        public void Use(User user)
        {
            UserId = user.Id;
            UserName = user.UserName;
            ProfileImage = user.ProfileImage;
        }
    }
}
