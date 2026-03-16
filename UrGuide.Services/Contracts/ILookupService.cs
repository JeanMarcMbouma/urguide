using System.Collections.Generic;
using System.Threading;
using BbQ.Outcome;
using System.Threading.Tasks;
using UrGuide.Model.Lookup;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    public interface ILookupService
    {
        Task<Outcome<IEnumerable<CategoryModel>>> GetCategoriesAsync(CancellationToken cancellationToken);
        Task<Outcome<IEnumerable<RegionModel>>> GetRegionsAsync(CancellationToken cancellationToken);
    }
}
