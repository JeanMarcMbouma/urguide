using System.Threading;
using BbQ.Outcome;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Tour;

namespace UrGuide.Services.Contracts
{
    public interface ITourRequestService
    {
        Task<Outcome<TourRequestModel>> CreateTourRequestAsync(CreateTourRequestModel model, CancellationToken cancellationToken);
        Task<Outcome<TourRequestModel>> GetTourRequestByIdAsync(string tourRequestId, CancellationToken cancellationToken);
        Task<Outcome<PagedList<TourRequestModel>>> GetTourRequestsAsync(SearchParameters pagination, CancellationToken cancellationToken);
        Task<Outcome<PagedList<TourRequestModel>>> GetMyTourRequestsAsync(SearchParameters pagination, CancellationToken cancellationToken);
        Task<Outcome<PagedList<TourRequestModel>>> GetTourRequestsByRegionAsync(string regionId, SearchParameters pagination, CancellationToken cancellationToken);
        Task<Outcome<bool>> CancelTourRequestAsync(string tourRequestId, CancellationToken cancellationToken);
        Task<Outcome<TourRequestModel>> UpdateBudgetAsync(string tourRequestId, decimal newBudget, CancellationToken cancellationToken);
    }
}