using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Tour;

namespace UrGuide.Services.Contracts
{
    public interface ITourRequestService
    {
        Task<Result<TourRequestModel>> CreateTourRequestAsync(CreateTourRequestModel model, CancellationToken cancellationToken);
        Task<Result<TourRequestModel>> GetTourRequestByIdAsync(string tourRequestId, CancellationToken cancellationToken);
        Task<Result<PagedList<TourRequestModel>>> GetTourRequestsAsync(SearchParameters pagination, CancellationToken cancellationToken);
        Task<Result<PagedList<TourRequestModel>>> GetMyTourRequestsAsync(SearchParameters pagination, CancellationToken cancellationToken);
        Task<Result<PagedList<TourRequestModel>>> GetTourRequestsByRegionAsync(string regionId, SearchParameters pagination, CancellationToken cancellationToken);
        Task<Result<bool>> CancelTourRequestAsync(string tourRequestId, CancellationToken cancellationToken);
        Task<Result<TourRequestModel>> UpdateBudgetAsync(string tourRequestId, decimal newBudget, CancellationToken cancellationToken);
    }
}