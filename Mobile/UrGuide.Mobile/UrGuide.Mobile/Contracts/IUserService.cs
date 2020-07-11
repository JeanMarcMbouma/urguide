using System;
using System.Collections.Generic;
using System.Text;
using UrGuide.Model.Users;

namespace UrGuide.Mobile.Contracts
{
    public interface IUserService
    {
        UserInfo GetUserInfo(string id = null);
    }
}
