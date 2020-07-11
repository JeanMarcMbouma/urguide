using UrGuide.Model.Results;
using UrGuide.Model.Users;

namespace UrGuide.Mobile.Contracts
{
    public interface IUserService
    {
        bool IsAuthenticated { get; }
        bool IsGuide { get; }
        User CurrentUser { get; set; }
        UserInfo GetUserInfo(string id = null);
        Result<bool> ChangePassword(ChangePasswordModel model);
        Result<bool> SaveProfile(UpdateGuideModel model);
    }
}
