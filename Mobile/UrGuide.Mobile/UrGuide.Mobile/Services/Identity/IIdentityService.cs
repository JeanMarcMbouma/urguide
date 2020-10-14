using System.Threading.Tasks;

namespace UrGuide.Mobile.Services.Identity
{
    public interface IIdentityService
    {
        Task SignInAsync();
        Task LogoutAsync();
        Task GetUserInfo();
    }
}
