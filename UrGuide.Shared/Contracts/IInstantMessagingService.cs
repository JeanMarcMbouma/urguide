using System.Threading.Tasks;

namespace UrGuide.Shared.Contracts
{
    public interface IInstantMessagingService
    {
        Task Send<T>(string userId, T message);
        Task Send<T, TUserInfo>(string userId, T message, TUserInfo userInfo);
    }
}
