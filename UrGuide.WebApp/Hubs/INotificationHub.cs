using System.Threading.Tasks;

namespace UrGuide.WebApp.Hubs
{
    public interface INotificationHub
    {
        Task NewChatMessage(string userId, string message);
    }
}