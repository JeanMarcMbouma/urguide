using System.Net;
using System.Threading.Tasks;

namespace UrGuide.Shared.Contracts
{
    public interface IUserContext
    {
        string UserId { get; }
        Task<string> Id_Token { get; }
        Task<string> Access_Token { get; }
        bool IsAuthenticated { get; }
        IPAddress IPAddress { get; }
    }
}
