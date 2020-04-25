using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public string UserId => string.Empty;

        public string UserName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Id_Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string ProfileImage => throw new NotImplementedException();

        public IHttpContextAccessor HttpContextAccessor { get; }

        public IUrlHelper Url { get; }

        public string ResolveUrl(MessageTypes confirmation, object parameters)
        {
            switch (confirmation)
            {
                case MessageTypes.Confirmation:
                    return Url.ActionLink("ConfirmEmail", "Account", parameters);
                case MessageTypes.PasswordReset:
                    break;
                case MessageTypes.ChangePassword:
                    break;
                default:
                    break;
            }
            throw new NotImplementedException();
        }
    }
}
