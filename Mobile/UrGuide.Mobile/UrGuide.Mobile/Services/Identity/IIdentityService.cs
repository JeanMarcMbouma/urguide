using IdentityModel.OidcClient;
using System.Threading.Tasks;

namespace UrGuide.Mobile.Services.Identity
{
    public interface IIdentityService
    {
        Task<LoginResult> SignInAsync();
    }
}
