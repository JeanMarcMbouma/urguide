using Sharpnado.Presentation.Forms.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrGuide.Mobile.Models;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;
using UrGuide.Model.Users;

namespace UrGuide.Mobile.Contracts
{
    public interface IUserService
    {
        Task<UserInfo> GetUserInfo(string id = null);
        Result<bool> ChangePassword(ChangePasswordModel model);
        Result<bool> SaveProfile(UpdateGuideModel model);
        Task<PageResult<AuthoredFeedback>> GetUserFeedback(string userId, int pageNumber);
        Task<IEnumerable<GalleryItem>> GetGalleryItems(string userId);

    }
}
