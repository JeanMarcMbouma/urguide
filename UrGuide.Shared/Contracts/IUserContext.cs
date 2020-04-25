using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Shared.Contracts
{
    public interface IUserContext
    {
        string UserId { get; }
        string UserName { get; set; }
        string Id_Token { get; set; }
        string ProfileImage { get; }
    }
}
