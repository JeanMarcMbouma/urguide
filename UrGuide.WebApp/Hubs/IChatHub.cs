using System.Threading.Tasks;
using UrGuide.Model.Messages;

namespace UrGuide.WebApp.Hubs
{
    public interface IChatHub
    {
        Task ReceiveMessage(ChatMessageDto message);
        Task UserTyping(string conversationId, string userId, string userName);
        Task UserStoppedTyping(string conversationId, string userId);
        Task MessageRead(string conversationId, string messageId, string readByUserId);
        Task UserOnline(string userId);
        Task UserOffline(string userId);
        Task FileShared(string conversationId, FileAttachmentDto file);
    }
}
