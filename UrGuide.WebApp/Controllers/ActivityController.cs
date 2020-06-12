using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model;
using UrGuide.Model.Auditing;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class ActivityController : Controller
    {
        public ActivityController(IUserActivityService ActivityService)
        {
            ActivityService = ActivityService ?? throw new ArgumentNullException(nameof(ActivityService));
        }

        public IUserActivityService ActivityService { get; }

        
        [HttpGet("all")]
        [ProducesDefaultResponseType(typeof(PagedList<ActivityModel>))]
        public async Task<IActionResult> GetUserActivity([FromQuery][Bind(nameof(PaginationParameters.PageNumber))]PaginationParameters pagination, CancellationToken cancellationToken)
        {
            var result = await ActivityService.GetUserActivityAsync(pagination, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }
    }
}
