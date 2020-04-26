using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Messages;

namespace UrGuide.Shared.Contracts
{
    public interface IEmailService
    {
        Task SendAsync(SendDirectMessageCommand message);
    }
}