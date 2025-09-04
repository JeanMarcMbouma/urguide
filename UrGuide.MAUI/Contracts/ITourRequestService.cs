using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrGuide.MAUI.Models;
using UrGuide.Model.Results;

namespace UrGuide.MAUI.Contracts
{
    public interface ITourRequestService
    {
        Task<Result<TourRequestItem>> CreateTourRequestAsync(UrGuide.MAUI.Models.API.CreateTourRequestModel model);
        Task<Result<IEnumerable<TourRequestItem>>> GetMyTourRequestsAsync(int pageNumber = 1);
        Task<Result<TourRequestItem>> GetTourRequestByIdAsync(string tourRequestId);
        Task<Result<TourRequestItem>> UpdateBudgetAsync(string tourRequestId, decimal newBudget);
        Task<Result<bool>> CancelTourRequestAsync(string tourRequestId);
        Task<Result<IEnumerable<UrGuide.MAUI.Models.API.RegionModel>>> GetRegionsAsync();
    }
}