using System.Threading.Tasks;

namespace UrGuide.Services.Contracts
{
    public interface IDataSeedingService
    {
        Task SeedDataAsync();
    }
}