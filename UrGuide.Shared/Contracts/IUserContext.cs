using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Shared.Contracts
{
    public interface IUserContext
    {
        string UserId { get; }
        string UserName { get; }
        string Id_Token { get; }
        string ProfileImage { get;  }
        bool IsAuthenticated { get; }

        string ResolveUrl(MessageTypes confirmation, object parameters);
        void Use(Model.Users.User user);
    }
}
