using System;
using UrGuide.Mobile.Contracts;
using UrGuide.Model.Results;
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
            ProfileImage = $"{Constants.BaseUrl}/images/85e526dd-6b92-4700-b427-6c7d7fe40a45.png",
        };

        static User _user = new User
        {
            BirthDay = "24-Oct-1984",
            City = "Cherkasy",
            Country = "Ukraine",
            Description = "I'm the awesome guide you've been looking for!",
            FirstName = "Jean Marc",
            FullName = "Jean Marc Mbouma",
            LastName = "Mbouma",
            Gender = "Male",
            Id = Guid.Empty.ToString("d"),
            IsGuide = false,
            IsPremium = true,
            PhoneNumber = "01-290203022",
            UserName = "jm@urguide.com",
            ProfileImage = $"{Constants.BaseUrl}/images/85e526dd-6b92-4700-b427-6c7d7fe40a45.png"
        };

        public bool IsAuthenticated => CurrentUser != null;

        public bool IsGuide => _user?.IsGuide ?? false;

        public User CurrentUser { get => _user; set => _user = value; }

        public Result<bool> ChangePassword(ChangePasswordModel model)
        {
            return Result.Of(false).WithErrors("Not yet implemented");
        }

        public UserInfo GetUserInfo(string id = null)
        {
            return user;
        }

        public Result<bool> SaveProfile(UpdateGuideModel model)
        {
            return Result.Of(false).WithErrors("Not yet implemented");
        }
    }
}
