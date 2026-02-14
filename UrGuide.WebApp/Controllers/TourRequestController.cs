using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Tour;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("api/tour-requests")]
    [Authorize]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class TourRequestController : ControllerBase
    {
        private readonly ITourRequestService _tourRequestService;

        public TourRequestController(ITourRequestService tourRequestService)
        {
            _tourRequestService = tourRequestService;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TourRequestModel))]
        public async Task<IActionResult> CreateTourRequest([FromBody] CreateTourRequestModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorEnvelop.Create(ModelState));

            var result = await _tourRequestService.CreateTourRequestAsync(model, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Created($"/tour-requests/{result.Data.TourRequestId}", result.Data);
        }

        [HttpGet("{tourRequestId}")]
        [ProducesResponseType(200, Type = typeof(TourRequestModel))]
        [AllowAnonymous]
        public async Task<IActionResult> GetTourRequest(string tourRequestId, CancellationToken cancellationToken)
        {
            var result = await _tourRequestService.GetTourRequestByIdAsync(tourRequestId, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PagedList<TourRequestModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetTourRequests([FromQuery] SearchParameters pagination, CancellationToken cancellationToken)
        {
            var result = await _tourRequestService.GetTourRequestsAsync(pagination, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpGet("my")]
        [ProducesResponseType(200, Type = typeof(PagedList<TourRequestModel>))]
        public async Task<IActionResult> GetMyTourRequests([FromQuery] SearchParameters pagination, CancellationToken cancellationToken)
        {
            var result = await _tourRequestService.GetMyTourRequestsAsync(pagination, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpGet("region/{regionId}")]
        [ProducesResponseType(200, Type = typeof(PagedList<TourRequestModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetTourRequestsByRegion(string regionId, [FromQuery] SearchParameters pagination, CancellationToken cancellationToken)
        {
            var result = await _tourRequestService.GetTourRequestsByRegionAsync(regionId, pagination, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPost("{tourRequestId}/cancel")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> CancelTourRequest(string tourRequestId, CancellationToken cancellationToken)
        {
            var result = await _tourRequestService.CancelTourRequestAsync(tourRequestId, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPut("{tourRequestId}/budget")]
        [ProducesResponseType(200, Type = typeof(TourRequestModel))]
        public async Task<IActionResult> UpdateBudget(string tourRequestId, [FromBody] UpdateBudgetModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorEnvelop.Create(ModelState));

            var result = await _tourRequestService.UpdateBudgetAsync(tourRequestId, model.NewBudget, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }
    }
}