using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Lookup;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    public interface ILookupService
    {
        Task<Result<IEnumerable<CategoryModel>>> GetCategoriesAsync(CancellationToken cancellationToken);
    }
}
