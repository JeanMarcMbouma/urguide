using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Auditing;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class ActivityController(IUserActivityService activityService) : Controller
    {

        [HttpGet("all")]
        [ProducesDefaultResponseType(typeof(PagedList<ActivityModel>))]
        public async Task<IActionResult> GetUserActivity([FromQuery][Bind(nameof(PaginationParameters.PageNumber))]PaginationParameters pagination, CancellationToken cancellationToken)
        {
            var result = await activityService.GetUserActivityAsync(pagination, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }
    }
}
