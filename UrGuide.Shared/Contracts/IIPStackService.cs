using System.Net;
using System.Threading.Tasks;

namespace UrGuide.Shared.Contracts
{
    public interface IIPStackService
    {
        Task<IPStackInfo> GetAsync(IPAddress ip);
    }
}
