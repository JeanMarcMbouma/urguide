using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using UrGuide.Model.Users;
using UrGuide.Shared;
using UrGuide.Shared.Contracts;

namespace UrGuide.WebApp.Services
{
    public class UserContext : IUserContext
    {
        public UserContext(IHttpContextAccessor httpContextAccessor, IUrlHelper url)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public IHttpContextAccessor HttpContextAccessor { get; }
        public IUrlHelper Url { get; }

        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string Id_Token { get; private set; }

        public string ProfileImage { get; private set; }

        public bool IsAuthenticated => HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

        public string ResolveUrl(MessageTypes confirmation, object parameters)
        {
            switch (confirmation)
            {
                case MessageTypes.Confirmation:
                    return Url.ActionLink("ConfirmEmail", "Account", parameters);
                case MessageTypes.PasswordReset:
                    return Url.Action("pforget", parameters);
                case MessageTypes.ChangePassword:
                    break;
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public void Use(User user)
        {
            UserId = user.Id;
            UserName = user.UserName;
            ProfileImage = user.ProfileImage;
        }
    }
}
