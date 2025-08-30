using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Lookup;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class LookupController : Controller
    {
        public LookupController(ILookupService lookupService, IUserService userService)
        {
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public ILookupService LookupService { get; }
        public IUserService UserService { get; }

        [HttpGet("categories")]
        [ProducesDefaultResponseType(typeof(IEnumerable<CategoryModel>))]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var result = await LookupService.GetCategoriesAsync(cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("regions")]
        [ProducesDefaultResponseType(typeof(IEnumerable<RegionModel>))]
        public async Task<IActionResult> GetRegions(CancellationToken cancellationToken)
        {
            var result = await LookupService.GetRegionsAsync(cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("/users/{id}/info")]
        [ProducesDefaultResponseType(typeof(UserInfo))]
        public async Task<IActionResult> GetOne(string id, CancellationToken cancellationToken)
        {
            var result = await UserService.GetUserInfo(id, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpGet("/users/search")]
        [ProducesDefaultResponseType(typeof(PagedList<UserInfo>))]
        public async Task<IActionResult> GetUsers([FromQuery]SearchParameters searchParameters, CancellationToken cancellationToken)
        {
            var result = await UserService.GetUsersAsync(searchParameters, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }
    }
}
