using Microsoft.AspNetCore.Http;
using System;
using UrGuide.Shared.Contracts;

namespace UrGuide.WebApp.Services
{
    public class UserContext : IUserContext
    {
        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public string UserId => string.Empty;

        public string UserName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Id_Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string ProfileImage => throw new NotImplementedException();

        public IHttpContextAccessor HttpContextAccessor { get; }
    }
}
