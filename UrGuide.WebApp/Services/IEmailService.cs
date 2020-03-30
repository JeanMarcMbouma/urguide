using System.Threading;
using System.Threading.Tasks;

namespace UrGuide.WebApp.Services
{
    public interface IEmailService
    {
        Task Send(string email, string messageBody, EmailService.MessageTypes messageType, CancellationToken cancellationToken);
    }
}