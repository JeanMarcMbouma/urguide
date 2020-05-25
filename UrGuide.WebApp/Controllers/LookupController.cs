using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Lookup;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class LookupController : Controller
    {
        public LookupController(ILookupService lookupService)
        {
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
        }

        public ILookupService LookupService { get; }

        [HttpGet("categories")]
        [ProducesDefaultResponseType(typeof(IEnumerable<CategoryModel>))]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var result = await LookupService.GetCategoriesAsync(cancellationToken);
            return Ok(result.Data);
        }
    }
}
