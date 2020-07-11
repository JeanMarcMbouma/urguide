using System;
using System.Collections.Generic;
using System.Text;
using UrGuide.Mobile.Contracts;
using UrGuide.Model.Users;

namespace UrGuide.Mobile.Services
{
    class UserService : IUserService
    {
        static UserInfo user = new UserInfo
        {
            City = "Yaounde",
            Country = "Cameroon",
            Description = "I am the guide you were looking for.",
            Rating = 4,
            FirstName = "Jean Marc",
            LastName = "Mbouma",
            FullName = "Jean Marc Mbouma",
            ProfileImage = "http://urguide.azurewebsites.net/images/85e526dd-6b92-4700-b427-6c7d7fe40a45.png",
        };
        public UserInfo GetUserInfo(string id = null)
        {
            return user;
        }
    }
}
